using ImoSphere.Data;
using ImoSphere.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ImoSphere.Controllers
{
    [Authorize(Roles = "Admin,Seller,User")] 
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PropertiesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: List all properties
        public async Task<IActionResult> Index()
        {
            var properties = await _context.Properties.ToListAsync();
            return View(properties);
        }

        // GET: View property details
        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound("Property not found.");
            }

            return View(property);
        }

        // GET: Create a new property
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create a new property
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> Create(Property property)
        {
            if (!ModelState.IsValid)
            {
                return View(property);
            }

            _context.Add(property);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Property created successfully.";
            return RedirectToAction("Properties", "Home");
        }

        // GET: Edit a property
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound("Property not found.");
            }

            return View(property);
        }

        // POST: Edit a property
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (id != property.Id)
            {
                return BadRequest("Invalid property ID.");
            }

            if (!ModelState.IsValid)
            {
                return View(property);
            }

            try
            {
                _context.Update(property);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Property updated successfully.";
                return RedirectToAction("Properties", "Home");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Properties.AnyAsync(e => e.Id == property.Id))
                {
                    return NotFound("Property not found.");
                }
                throw;
            }
        }

        // POST: Delete a property
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound("Property not found.");
            }

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Property deleted successfully.";
            return RedirectToAction("Properties", "Home");
        }
    }
}