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

    public async Task<IActionResult> Properties(
        decimal? minPrice = null, decimal? maxPrice = null,
        int? minBathrooms = null, int? maxBathrooms = null,
        int? minBedrooms = null, int? maxBedrooms = null,
        int? minArea = null, int? maxArea = null,
        int? minYearBuilt = null, int? maxYearBuilt = null,
        string location = null, List<int> agencyIds = null,
        string sortBy = "Name", string sortOrder = "asc",
        bool showMap = false)
    {
        var query = _context.Properties
            .Include(p => p.Images)
            .Include(p => p.Agency)
            .AsQueryable();

        // Filtros
        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);
        if (minBathrooms.HasValue)
            query = query.Where(p => p.Bathrooms >= minBathrooms.Value);
        if (maxBathrooms.HasValue)
            query = query.Where(p => p.Bathrooms <= maxBathrooms.Value);
        if (minBedrooms.HasValue)
            query = query.Where(p => p.Bedrooms >= minBedrooms.Value);
        if (maxBedrooms.HasValue)
            query = query.Where(p => p.Bedrooms <= maxBedrooms.Value);
        if (minArea.HasValue)
            query = query.Where(p => p.Area >= minArea.Value);
        if (maxArea.HasValue)
            query = query.Where(p => p.Area <= maxArea.Value);
        if (minYearBuilt.HasValue)
            query = query.Where(p => p.YearBuilt >= minYearBuilt.Value);
        if (maxYearBuilt.HasValue)
            query = query.Where(p => p.YearBuilt <= maxYearBuilt.Value);
        if (!string.IsNullOrEmpty(location))
            query = query.Where(p => p.Location.Contains(location));
        if (agencyIds != null && agencyIds.Any())
            query = query.Where(p => agencyIds.Contains(p.AgencyId));

        var totalResults = await _context.Properties.CountAsync();
        var availableAgencies = await _context.Agencies.Where(a => a.Name != "ImoSphere").ToListAsync();
        var properties = await query.ToListAsync();

        // Corrigir ordenação para SQLite (Price e Area em memória)
        switch (sortBy.ToLower())
        {
            case "price":
                properties = sortOrder == "asc" ? properties.OrderBy(p => p.Price).ToList() : properties.OrderByDescending(p => p.Price).ToList();
                break;
            case "area":
                properties = sortOrder == "asc" ? properties.OrderBy(p => p.Area).ToList() : properties.OrderByDescending(p => p.Area).ToList();
                break;
            case "bedrooms":
                properties = sortOrder == "asc" ? properties.OrderBy(p => p.Bedrooms).ToList() : properties.OrderByDescending(p => p.Bedrooms).ToList();
                break;
            case "bathrooms":
                properties = sortOrder == "asc" ? properties.OrderBy(p => p.Bathrooms).ToList() : properties.OrderByDescending(p => p.Bathrooms).ToList();
                break;
            case "yearbuilt":
                properties = sortOrder == "asc" ? properties.OrderBy(p => p.YearBuilt).ToList() : properties.OrderByDescending(p => p.YearBuilt).ToList();
                break;
            default:
                properties = sortOrder == "asc" ? properties.OrderBy(p => p.Name).ToList() : properties.OrderByDescending(p => p.Name).ToList();
                break;
        }

        // User info
        string userAgency = null;
        string userRole = null;
        string userId = null;
        List<string> supervisedComercialIds = null;
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
                userId = user.Id;
                
                // Se for Admin, buscar os IDs dos comerciais que supervisiona
                if (userRole == "Admin" && agencyUser != null)
                {
                    supervisedComercialIds = await _context.AgencyUsers
                        .Where(au => au.AgencyId == agencyUser.AgencyId && au.Role == "Comercial" && au.AdminId == user.Id)
                        .Select(au => au.UserId)
                        .ToListAsync();
                }
            }
        }

        var filterModel = new PropertyFilterViewModel
        {
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            MinBathrooms = minBathrooms,
            MaxBathrooms = maxBathrooms,
            MinBedrooms = minBedrooms,
            MaxBedrooms = maxBedrooms,
            MinArea = minArea,
            MaxArea = maxArea,
            MinYearBuilt = minYearBuilt,
            MaxYearBuilt = maxYearBuilt,
            Location = location,
            AgencyIds = agencyIds ?? new List<int>(),
            SortBy = sortBy,
            SortOrder = sortOrder,
            Properties = properties,
            AvailableAgencies = availableAgencies,
            TotalResults = totalResults,
            FilteredResults = properties.Count,
            ShowMap = showMap
        };

        ViewBag.UserAgency = userAgency;
        ViewBag.UserRole = userRole;
        ViewBag.UserId = userId;
        ViewBag.SupervisedComercialIds = supervisedComercialIds;
        
        return View(filterModel);
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
            // Admin: lista apenas comerciais supervisionados por ele
            var comerciais = await _context.AgencyUsers
                .Where(au => au.AgencyId == agencyUser.AgencyId && au.Role == "Comercial" && au.AdminId == user.Id)
                .Include(au => au.User)
                .ToListAsync();
            var comerciaisComCasas = new List<(ApplicationUser Comercial, List<Property> Casas)>();
            var userRolesList = new List<UserWithRolesViewModel>();
            foreach (var comercial in comerciais)
            {
                var casas = await _context.Properties
                    .Where(p => p.CreatedByUserId == comercial.UserId)
                    .Include(p => p.Images)
                    .Include(p => p.Agency)
                    .ToListAsync();
                comerciaisComCasas.Add((comercial.User, casas));
                var userRoles = await _userManager.GetRolesAsync(comercial.User);
                userRolesList.Add(new UserWithRolesViewModel { User = comercial.User, Roles = userRoles });
            }
            ViewBag.Agency = agencyUser.Agency?.Name;
            ViewBag.ComerciaisComCasas = comerciaisComCasas;
            ViewBag.UserList = userRolesList;
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
