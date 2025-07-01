using ImoSphere.Data;
using ImoSphere.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;

namespace ImoSphere.Controllers
{
    [Authorize(Roles = "Admin,Comercial,User")] 
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PropertiesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: List all properties
        public async Task<IActionResult> Index()
        {
            var properties = await _context.Properties.Include(p => p.Images).Include(p => p.Agency).ToListAsync();
            return View(properties);
        }

        // GET: View property details
        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties.Include(p => p.Images).Include(p => p.Agency).FirstOrDefaultAsync(p => p.Id == id);
            if (property == null)
            {
                return NotFound("Property not found.");
            }
            return View(property);
        }

        // GET: Create a new property
        [Authorize(Roles = "Admin,Comercial")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create a new property
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Comercial")]
        public async Task<IActionResult> Create(Property property, List<IFormFile> images)
        {
            if (!ModelState.IsValid)
            {
                return View(property);
            }

            var user = await _userManager.GetUserAsync(User);
            var agencyUser = _context.AgencyUsers.FirstOrDefault(au => au.UserId == user.Id);
            if (agencyUser == null)
            {
                return Forbid();
            }
            property.AgencyId = agencyUser.AgencyId;
            property.Images = new List<PropertyImage>();
            if (images != null && images.Count > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                Directory.CreateDirectory(uploadPath);
                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        property.Images.Add(new PropertyImage { ImageUrl = "/images/" + fileName });
                    }
                }
            }
            _context.Add(property);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Property created successfully.";
            return RedirectToAction("Properties", "Home");
        }

        // GET: Edit a property
        [Authorize(Roles = "Admin,Comercial")]
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _context.Properties.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (property == null)
            {
                return NotFound("Property not found.");
            }
            var user = await _userManager.GetUserAsync(User);
            var agencyUser = _context.AgencyUsers.FirstOrDefault(au => au.UserId == user.Id);
            if (agencyUser == null || property.AgencyId != agencyUser.AgencyId)
            {
                return Forbid();
            }
            return View(property);
        }

        // POST: Edit a property
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Comercial")]
        public async Task<IActionResult> Edit(int id, Property property, List<IFormFile> images)
        {
            if (id != property.Id)
            {
                return BadRequest("Invalid property ID.");
            }
            if (!ModelState.IsValid)
            {
                return View(property);
            }
            var user = await _userManager.GetUserAsync(User);
            var agencyUser = _context.AgencyUsers.FirstOrDefault(au => au.UserId == user.Id);
            if (agencyUser == null)
            {
                return Forbid();
            }
            var existingProperty = await _context.Properties.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (existingProperty == null || existingProperty.AgencyId != agencyUser.AgencyId)
            {
                return Forbid();
            }
            try
            {
                // Atualizar campos básicos
                existingProperty.Name = property.Name;
                existingProperty.Description = property.Description;
                existingProperty.Price = property.Price;
                existingProperty.Bedrooms = property.Bedrooms;
                existingProperty.Bathrooms = property.Bathrooms;
                existingProperty.Area = property.Area;
                existingProperty.Location = property.Location;
                existingProperty.YearBuilt = property.YearBuilt;
                // Upload de novas imagens
                if (images != null && images.Count > 0)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    Directory.CreateDirectory(uploadPath);
                    foreach (var image in images)
                    {
                        if (image.Length > 0)
                        {
                            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                            var filePath = Path.Combine(uploadPath, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }
                            existingProperty.Images.Add(new PropertyImage { ImageUrl = "/images/" + fileName });
                        }
                    }
                }
                _context.Update(existingProperty);
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
        [Authorize(Roles = "Admin,Comercial")]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound("Property not found.");
            }
            var user = await _userManager.GetUserAsync(User);
            var agencyUser = _context.AgencyUsers.FirstOrDefault(au => au.UserId == user.Id);
            if (agencyUser == null || property.AgencyId != agencyUser.AgencyId)
            {
                return Forbid();
            }
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Property deleted successfully.";
            return RedirectToAction("Properties", "Home");
        }
    }
}