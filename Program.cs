using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ImoSphere.Data;
using ImoSphere.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar o contexto do banco de dados com SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar os serviços do Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configurar os controladores com views
builder.Services.AddControllersWithViews();

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

app.UseAuthentication();  // Certifique-se de que está usando autenticação
app.UseAuthorization();   // E também autorização

// Mapear as rotas padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seeding de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    await SeedData.Initialize(services, context);  // Chamando o método de seeding diretamente pela classe
}

app.Run();
