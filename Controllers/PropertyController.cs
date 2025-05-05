using ImoSphere.Data;
using ImoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ImoSphere.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Construtor com injeção de dependência
        public PropertiesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Verifica se o usuário é vendedor
        private async Task<bool> IsUserSellerAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                return user?.Email.EndsWith("@seller.com") ?? false;
            }
            return false;
        }

        // Action para exibir a lista de propriedades
        public async Task<IActionResult> Index()
        {
            var properties = await _context.Properties.ToListAsync();

            // Verifica se o usuário está autenticado e se é um vendedor
            var isSeller = await IsUserSellerAsync();

            // Passa a informação para a view
            ViewBag.IsSeller = isSeller;
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

            return View(properties);
        }

        // Action para mostrar detalhes de uma propriedade
        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            // Verificar se o usuário é vendedor
            var isSeller = await IsUserSellerAsync();

            ViewBag.IsSeller = isSeller;

            return View(property);
        }

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]
public async Task<IActionResult> Edit(int id, Property property)
{
    if (id != property.Id)
    {
        return NotFound();
    }

    var isSeller = await IsUserSellerAsync();
    if (!isSeller)
    {
        return RedirectToAction("Index", "Properties");
    }

    if (ModelState.IsValid)
    {
        try
        {
            _context.Update(property);
            await _context.SaveChangesAsync();
            // Adiciona uma mensagem de sucesso para exibir no popup
            TempData["SuccessMessage"] = "Changes saved successfully!";
            return RedirectToAction(nameof(Edit), new { id = property.Id });  // Redireciona de volta para a página de edição
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Properties.AnyAsync(e => e.Id == property.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    return View(property);
}

        // GET: Properties/Edit/5
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var isSeller = await IsUserSellerAsync();
            if (!isSeller)
            {
               return RedirectToAction("Properties", "Home");
            }

            return View(property);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var isSeller = await IsUserSellerAsync();
            if (!isSeller)
            {
                return RedirectToAction("Properties", "Home");
            }

            return View();
        }

// POST: Properties/Create
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]
public async Task<IActionResult> Create(Property property)
{
    // Verifica se o usuário é um vendedor
    var isSeller = await IsUserSellerAsync();
    if (!isSeller)
    {
        return RedirectToAction("Properties", "Home");
    }

    if (ModelState.IsValid)
    {
        _context.Update(property);
            await _context.SaveChangesAsync();
            // Adiciona uma mensagem de sucesso para exibir no popup
            TempData["SuccessMessage"] = "Changes saved successfully!";
            return RedirectToAction(nameof(Edit), new { id = property.Id });  // Redireciona de volta para a página de edição
    }

    return View(property);
}
    }
}



