using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImoSphere.Data;
using Microsoft.AspNetCore.Authorization;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    // INJEÇÃO DE DEPENDÊNCIA: recebe o contexto pelo construtor
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AboutUs()
    {
        return View();
    }

    // PROPERTIES: busca da BD
    public async Task<IActionResult> Properties()
    {
        var properties = await _context.Properties.ToListAsync();
        return View(properties); // <- envia para a view
    }

    public IActionResult Services()
    {
        return View();
    }

    public IActionResult ContactUs()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AdminUser()
    {
        return View();
    }
}
