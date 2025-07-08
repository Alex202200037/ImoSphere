using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ImoSphere.Data;
using ImoSphere.Models;
using ImoSphere.Hubs;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configurar o contexto do banco de dados com SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar os serviços do Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ áàãâéèêíìîóòõôúùûçÁÀÃÂÉÈÊÍÌÎÓÒÕÔÚÙÛÇ";
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configurar localização
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Configurar culturas suportadas
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("pt"),
        new CultureInfo("en"),
        new CultureInfo("es")
    };

    options.DefaultRequestCulture = new RequestCulture("pt");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configurar a pipeline de middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Adicionar middleware de localização
app.UseRequestLocalization();

app.UseAuthentication();  // Certifique-se de que está usando autenticação
app.UseAuthorization();   // E também autorização

// Mapear as rotas padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub");

// Seeding de dados - CORRIGIDO: Migrations primeiro, depois seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        Console.WriteLine("[STARTUP] Aplicando migrações...");
        context.Database.Migrate();
        Console.WriteLine("[STARTUP] Migrações aplicadas com sucesso.");
        
        Console.WriteLine("[STARTUP] Iniciando seed de dados...");
        await SeedData.Initialize(services, context);
        Console.WriteLine("[STARTUP] Seed de dados concluído.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[STARTUP] Erro durante inicialização: {ex.Message}");
        throw;
    }
}

app.Run();
