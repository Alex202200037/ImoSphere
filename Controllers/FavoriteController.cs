using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ImoSphere.Data;
using ImoSphere.Models;
using System.Security.Claims;

namespace ImoSphere.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoriteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: /Favorite/ToggleFavorite
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int propertyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Utilizador não autenticado" });
            }

            // Verificar se a propriedade existe
            var property = await _context.Properties
                .Include(p => p.Agency)
                .FirstOrDefaultAsync(p => p.Id == propertyId);

            if (property == null)
            {
                return Json(new { success = false, message = "Propriedade não encontrada" });
            }

            // Verificar se já é favorito
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PropertyId == propertyId);

            if (existingFavorite != null)
            {
                // Remover dos favoritos
                _context.Favorites.Remove(existingFavorite);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = false, message = "Removido dos favoritos" });
            }
            else
            {
                // Adicionar aos favoritos
                var favorite = new Favorite
                {
                    UserId = userId,
                    PropertyId = propertyId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = true, message = "Adicionado aos favoritos" });
            }
        }

        // GET: /Favorite/GetFavorites
        public async Task<IActionResult> GetFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var favorites = await _context.Favorites
                .Include(f => f.Property)
                    .ThenInclude(p => p.Agency)
                .Include(f => f.Property)
                    .ThenInclude(p => p.Images)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return View(favorites);
        }

        // POST: /Favorite/RemoveFavorite
        [HttpPost]
        public async Task<IActionResult> RemoveFavorite(int favoriteId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Utilizador não autenticado" });
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.Id == favoriteId && f.UserId == userId);

            if (favorite == null)
            {
                return Json(new { success = false, message = "Favorito não encontrado" });
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Favorito removido" });
        }

        // GET: /Favorite/CheckFavorite
        [HttpGet]
        public async Task<IActionResult> CheckFavorite(int propertyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { isFavorite = false });
            }

            var isFavorite = await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.PropertyId == propertyId);

            return Json(new { isFavorite });
        }
    }
} 