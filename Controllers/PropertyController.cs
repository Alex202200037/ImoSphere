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
using Microsoft.AspNetCore.Mvc.Rendering;

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
            // Remover erros de validação destes campos antes de validar (igual ao Edit)
            ModelState.Remove("Agency");
            ModelState.Remove("property.Agency");
            ModelState.Remove("Images");
            ModelState.Remove("property.Images");
            ModelState.Remove("CreatedByUser");
            ModelState.Remove("property.CreatedByUser");
            ModelState.Remove("CreatedByUserId");
            ModelState.Remove("property.CreatedByUserId");
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
            property.CreatedByUserId = user.Id;
            property.CreatedByUser = user;
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
            var property = await _context.Properties.Include(p => p.Images).Include(p => p.Agency).FirstOrDefaultAsync(p => p.Id == id);
            if (property == null)
            {
                return NotFound("Property not found.");
            }
            var user = await _userManager.GetUserAsync(User);
            var agencyUser = _context.AgencyUsers.FirstOrDefault(au => au.UserId == user.Id);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            ViewBag.UserRole = isAdmin ? "Admin" : "Comercial";
            if (isAdmin)
            {
                // Lista de comerciais da agência como SelectList
                var comerciais = await _context.AgencyUsers
                    .Where(au => au.AgencyId == agencyUser.AgencyId && au.Role == "Comercial")
                    .Select(au => au.User)
                    .ToListAsync();
                ViewBag.Comerciais = new SelectList(comerciais, "Id", "UserName", property.CreatedByUserId);
            }
            if (agencyUser == null || property.AgencyId != agencyUser.AgencyId || (!isAdmin && property.CreatedByUserId != user.Id))
            {
                return Forbid();
            }
            return View(property);
        }

        // POST: Edit a property
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Comercial")]
        public async Task<IActionResult> Edit(int id, Property property, List<IFormFile> images, List<int> removeImageIds)
        {
            if (id != property.Id)
            {
                return BadRequest("Invalid property ID.");
            }
            ModelState.Remove("Agency");
            ModelState.Remove("property.Agency");
            // Carregar a propriedade original antes de permissões/validação
            var existingProperty = await _context.Properties.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (existingProperty == null)
            {
                return NotFound("Property not found.");
            }
            var user = await _userManager.GetUserAsync(User);
            var agencyUser = _context.AgencyUsers.FirstOrDefault(au => au.UserId == user.Id);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (agencyUser == null)
            {
                return Forbid();
            }
            // Usar os dados da propriedade original para permissões
            if (existingProperty.AgencyId != agencyUser.AgencyId || (!isAdmin && existingProperty.CreatedByUserId != user.Id))
            {
                return Forbid();
            }
            // Remover sempre os erros de validação destes campos antes de validar
            ModelState.Remove("CreatedByUser");
            ModelState.Remove("property.CreatedByUser");
            ModelState.Remove("CreatedByUserId");
            ModelState.Remove("property.CreatedByUserId");
            if (!ModelState.IsValid)
            {
                if (isAdmin)
                {
                    var comerciais = await _context.AgencyUsers
                        .Where(au => au.AgencyId == agencyUser.AgencyId && au.Role == "Comercial")
                        .Select(au => au.User)
                        .ToListAsync();
                    ViewBag.Comerciais = new SelectList(comerciais, "Id", "UserName");
                }
                ViewBag.UserRole = isAdmin ? "Admin" : "Comercial";
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(property);
            }
            try
            {
                // Atualizar campos editáveis
                existingProperty.Name = property.Name;
                existingProperty.Description = property.Description;
                existingProperty.Price = property.Price;
                existingProperty.Bedrooms = property.Bedrooms;
                existingProperty.Bathrooms = property.Bathrooms;
                existingProperty.Area = property.Area;
                existingProperty.Location = property.Location;
                existingProperty.YearBuilt = property.YearBuilt;
                existingProperty.AgencyId = agencyUser.AgencyId;
                // Só o admin pode alterar o comercial responsável
                if (isAdmin)
                {
                    existingProperty.CreatedByUserId = property.CreatedByUserId;
                    existingProperty.CreatedByUser = await _context.ApplicationUsers.FindAsync(property.CreatedByUserId);
                }
                else
                {
                    existingProperty.CreatedByUserId = user.Id;
                    existingProperty.CreatedByUser = user;
                }

                // Remover imagens marcadas
                if (removeImageIds != null && removeImageIds.Count > 0)
                {
                    var imagesToRemove = existingProperty.Images.Where(img => removeImageIds.Contains(img.Id)).ToList();
                    foreach (var img in imagesToRemove)
                    {
                        _context.PropertyImages.Remove(img);
                    }
                }

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
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (agencyUser == null || property.AgencyId != agencyUser.AgencyId || (!isAdmin && property.CreatedByUserId != user.Id))
            {
                return Forbid();
            }
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Property deleted successfully.";
            return RedirectToAction("Properties", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Comercial")]
        public async Task<IActionResult> RemoveImage(int propertyId, int imageId)
        {
            var user = await _userManager.GetUserAsync(User);
            var agencyUser = _context.AgencyUsers.FirstOrDefault(au => au.UserId == user.Id);
            var property = await _context.Properties.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == propertyId);
            if (property == null || agencyUser == null || property.AgencyId != agencyUser.AgencyId)
            {
                return Forbid();
            }
            var image = property.Images.FirstOrDefault(i => i.Id == imageId);
            if (image != null)
            {
                _context.PropertyImages.Remove(image);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Edit", new { id = propertyId });
        }
    }
}