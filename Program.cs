using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ImoSphere.Data;
using ImoSphere.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Configure the database context with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add SignalR services
builder.Services.AddSignalR(); // Register SignalR services

// Configure controllers with views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map default routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map the SignalR hub
app.MapHub<ChatHub>("/chatHub");

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    await SeedData.Initialize(services, context);
}

app.Run();