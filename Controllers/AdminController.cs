using ImoSphere.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImoSphere.Controllers
{
    [Authorize(Roles = "Admin")] // Restrict access to Admins only
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Manage Users
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = roles
                });
            }

            return View(userRoles);
        }

        // GET: Create Seller or Admin
        [HttpGet]
        [Route("Admin/CreateUser")] // Explicit route for the GET action
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: Create Seller or Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/CreateUser")] // Explicit route for the POST action
        public async Task<IActionResult> CreateUser(string email, string username, string role, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email, username, role, and password are required.");
                return View();
            }
            // Check if the email already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "A user with this email already exists.");
                return View();
            }

            // Create the user
            var user = new IdentityUser
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Assign the role
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (roleResult.Succeeded)
                {
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