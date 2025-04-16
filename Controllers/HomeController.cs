using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{

    public IActionResult Index()
    {
        return View();
    }

    // Página "About Us"
    public IActionResult AboutUs()
    {
        return View();
    }

    // Página "Properties"
    public IActionResult Properties()
    {
        return View();
    }

    // Página "Services"
    public IActionResult Services()
    {
        return View();
    }

    // Página "Contact Us"
    public IActionResult ContactUs()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
