using ImoSphere.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ImoSphere.Data;
using Microsoft.EntityFrameworkCore;

namespace ImoSphere.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")] // Restrict access to Admins and SuperAdmins
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Manage Users
        public async Task<IActionResult> Users()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains("SuperAdmin");
            List<UserWithRolesViewModel> userRoles = new();
            if (isSuperAdmin)
            {
                var users = _userManager.Users.ToList();
                foreach (var user in users)
                {
                    var userRolesList = await _userManager.GetRolesAsync(user);
                    userRoles.Add(new UserWithRolesViewModel
                    {
                        User = user,
                        Roles = userRolesList
                    });
                }
            }
            else
            {
                // Só users da agência
                var agencyUser = await _context.AgencyUsers.FirstOrDefaultAsync(au => au.UserId == currentUser.Id);
                if (agencyUser == null)
                    return Forbid();
                var agencyUsers = _context.AgencyUsers.Where(au => au.AgencyId == agencyUser.AgencyId).Select(au => au.UserId).ToList();
                var users = _userManager.Users.Where(u => agencyUsers.Contains(u.Id)).ToList();
                foreach (var user in users)
                {
                    var userRolesList = await _userManager.GetRolesAsync(user);
                    userRoles.Add(new UserWithRolesViewModel
                    {
                        User = user,
                        Roles = userRolesList
                    });
                }
            }
            return View(userRoles);
        }

        // GET: Create Seller or Admin
        [HttpGet]
        [Route("Admin/CreateUser")]
        public async Task<IActionResult> CreateUser()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains("SuperAdmin");
            if (isSuperAdmin)
            {
                ViewBag.Agencies = _context.Agencies.ToList();
            }
            else
            {
                // Buscar agência do admin
                var agencyUser = await _context.AgencyUsers.Include(au => au.Agency).FirstOrDefaultAsync(au => au.UserId == currentUser.Id);
                if (agencyUser?.Agency == null)
                    return Forbid();
                var domain = $".{agencyUser.Agency.Name.ToLower()}@imosphere.com";
                ViewBag.EmailDomain = domain;
            }
            return View();
        }

        // POST: Create Seller or Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/CreateUser")]
        public async Task<IActionResult> CreateUser(string email, string emailPrefix, string username, string role, string password, int? agencyId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains("SuperAdmin");
            if (!isSuperAdmin)
            {
                // Buscar agência do admin
                var agencyUser = await _context.AgencyUsers.Include(au => au.Agency).FirstOrDefaultAsync(au => au.UserId == currentUser.Id);
                if (agencyUser?.Agency == null)
                    return Forbid();
                var domain = $".{agencyUser.Agency.Name.ToLower()}@imosphere.com";
                ViewBag.EmailDomain = domain;
                if (string.IsNullOrEmpty(emailPrefix))
                {
                    ModelState.AddModelError(string.Empty, "Email prefix is required.");
                    return View();
                }
                email = emailPrefix + domain;
            }
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email, username, role, and password are required.");
                if (isSuperAdmin) ViewBag.Agencies = _context.Agencies.ToList();
                return View();
            }
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "A user with this email already exists.");
                if (isSuperAdmin) ViewBag.Agencies = _context.Agencies.ToList();
                return View();
            }
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (roleResult.Succeeded)
                {
                    // Associação à agência
                    if (role == "Admin" || role == "Comercial" || role == "User")
                    {
                        int? agencyToAssign = null;
                        if (isSuperAdmin)
                        {
                            agencyToAssign = agencyId;
                        }
                        else
                        {
                            var agencyUser = await _context.AgencyUsers.FirstOrDefaultAsync(au => au.UserId == currentUser.Id);
                            if (agencyUser == null)
                                return Forbid();
                            agencyToAssign = agencyUser.AgencyId;
                        }
                        if (agencyToAssign.HasValue)
                        {
                            _context.AgencyUsers.Add(new AgencyUser
                            {
                                UserId = user.Id,
                                AgencyId = agencyToAssign.Value,
                                Role = role
                            });
                            await _context.SaveChangesAsync();
                        }
                        else if (!isSuperAdmin)
                        {
                            // Admin não pode criar user sem agência
                            ModelState.AddModelError(string.Empty, "Admins só podem criar utilizadores para a sua agência.");
                            return View();
                        }
                    }
                    else if (!isSuperAdmin)
                    {
                        // Admin não pode criar user sem agência
                        ModelState.AddModelError(string.Empty, "Admins só podem criar utilizadores para a sua agência.");
                        return View();
                    }
                    TempData["SuccessMessage"] = $"{role} created successfully.";
                    return RedirectToAction("Users");
                }
                else
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            if (isSuperAdmin) ViewBag.Agencies = _context.Agencies.ToList();
            return View();
        }

        public IActionResult ManageProperties()
        {
            return View();
        }

        [HttpGet]
        [Route("Admin/EditUser/{id}")]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID is required.");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Role = roles.FirstOrDefault()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Email = model.Email;
            user.UserName = model.UserName;

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "The new password and confirmation do not match.");
                    return View(model);
                }

                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    foreach (var error in removePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    foreach (var error in addPasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRoleAsync(user, currentRoles.First());
            if (removeResult.Succeeded)
            {
                var addResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction("Users");
            }

            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID is required.");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
                return RedirectToAction("Users");
            }

            TempData["ErrorMessage"] = "Failed to delete user.";
            return RedirectToAction("Users");
        }
    }
}