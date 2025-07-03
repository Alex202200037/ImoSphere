using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImoSphere.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ImoSphere.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    private static List<Message> _messages = new List<Message>();
    private static int _messageIdCounter = 1;
    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AboutUs()
    {
        return View();
    }

    public async Task<IActionResult> Properties()
    {
        var properties = await _context.Properties
            .Include(p => p.Images)
            .Include(p => p.Agency)
            .ToListAsync();

        string userAgency = null;
        string userRole = null;
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var agencyUser = await _context.AgencyUsers
                    .Where(au => au.UserId == user.Id)
                    .Include(au => au.Agency)
                    .FirstOrDefaultAsync();

                userAgency = agencyUser?.Agency?.Name;
                var roles = await _userManager.GetRolesAsync(user);
                userRole = roles.FirstOrDefault();
                ViewBag.UserId = user.Id;
            }
        }
        ViewBag.UserAgency = userAgency;
        ViewBag.UserRole = userRole;
        return View(properties); 
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
     [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitContactForm(string Name, string Email, string Message)
    {
        _messages.Add(new Message
        {
            Id = _messageIdCounter++, 
            Name = Name,
            Email = Email,
            Content = Message,
            IsRead = false
        });

        TempData["SuccessMessage"] = "Your message has been sent successfully!";
        return RedirectToAction("ContactUs");
    }

    [Authorize(Roles = "Admin")]
    public IActionResult ViewMessages()
    {
        return View(_messages);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult MarkAsRead(int id)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            message.IsRead = true;
        }

        return RedirectToAction("ViewMessages");
    }

    [Authorize(Roles = "Admin")]
    public IActionResult DeleteMessage(int id)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            _messages.Remove(message);
        }

        return RedirectToAction("ViewMessages");
    }
    [Authorize(Roles = "Admin")]
    public IActionResult AdminUser()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Perfil()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login", "Account");
        var roles = await _userManager.GetRolesAsync(user);
        var isAdmin = roles.Contains("Admin");
        var agencyUser = await _context.AgencyUsers.Include(au => au.Agency).FirstOrDefaultAsync(au => au.UserId == user.Id);
        if (isAdmin)
        {
            // Admin: lista comerciais da agência e casas de cada comercial
            var comerciais = await _context.AgencyUsers
                .Where(au => au.AgencyId == agencyUser.AgencyId && au.Role == "Comercial")
                .Include(au => au.User)
                .ToListAsync();
            var comerciaisComCasas = new List<(ApplicationUser Comercial, List<Property> Casas)>();
            foreach (var comercial in comerciais)
            {
                var casas = await _context.Properties.Where(p => p.CreatedByUserId == comercial.UserId).ToListAsync();
                comerciaisComCasas.Add((comercial.User, casas));
            }
            ViewBag.Agency = agencyUser.Agency?.Name;
            ViewBag.ComerciaisComCasas = comerciaisComCasas;
            return View("PerfilAdmin", user);
        }
        else
        {
            // Comercial: lista casas criadas por ele
            var casas = await _context.Properties.Where(p => p.CreatedByUserId == user.Id).ToListAsync();
            ViewBag.Agency = agencyUser.Agency?.Name;
            ViewBag.Casas = casas;
            return View("PerfilComercial", user);
        }
    }
}
