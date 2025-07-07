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
        public async Task<IActionResult> Users(int? agencyId = null, string? adminId = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains("SuperAdmin");
            
            if (isSuperAdmin)
            {
                // Para SuperAdmin: mostrar hierarquia organizada
                var hierarchy = await BuildUserHierarchy(agencyId, adminId);
                var filterModel = new UserFilterViewModel
                {
                    SelectedAgencyId = agencyId,
                    SelectedAdminId = adminId,
                    Agencies = await _context.Agencies.ToListAsync(),
                    Admins = await GetAdminsForFilter(agencyId)
                };
                // Buscar users sem agência
                var allUsers = _userManager.Users.ToList();
                var usersSemAgencia = new List<ApplicationUser>();
                foreach (var u in allUsers)
                {
                    var rolesU = await _userManager.GetRolesAsync(u);
                    var temAgencia = await _context.AgencyUsers.AnyAsync(au => au.UserId == u.Id);
                    if (rolesU.Count == 1 && rolesU.Contains("User") && !temAgencia)
                        usersSemAgencia.Add(u);
                }
                ViewBag.UsersSemAgencia = usersSemAgencia;
                ViewBag.FilterModel = filterModel;
                ViewBag.IsSuperAdmin = true;
                // Filtro especial para "sem agência"
                if (agencyId == -1)
                {
                    return View("UsersHierarchy", new List<UserHierarchyViewModel>()); // Model vazio, só mostra card especial
                }
                return View("UsersHierarchy", hierarchy);
            }
            else
            {
                // Para Admin: mostrar lista simples dos seus comerciais supervisionados
                var agencyUser = await _context.AgencyUsers.FirstOrDefaultAsync(au => au.UserId == currentUser.Id);
                if (agencyUser == null)
                    return Forbid();
                
                // Só os comerciais supervisionados por este admin
                var supervisedComerciais = _context.AgencyUsers
                    .Where(au => au.AdminId == currentUser.Id && au.Role == "Comercial")
                    .Select(au => au.UserId)
                    .ToList();

                var users = _userManager.Users.Where(u => supervisedComerciais.Contains(u.Id)).ToList();
                
                List<UserWithRolesViewModel> userRoles = new();
                foreach (var user in users)
                {
                    var userRolesList = await _userManager.GetRolesAsync(user);
                    userRoles.Add(new UserWithRolesViewModel
                    {
                        User = user,
                        Roles = userRolesList
                    });
                }
                
                ViewBag.IsSuperAdmin = false;
                return View("UsersSimple", userRoles);
            }
        }

        public async Task<List<UserHierarchyViewModel>> BuildUserHierarchy(int? agencyId = null, string? adminId = null)
        {
            var hierarchy = new List<UserHierarchyViewModel>();
            bool showImoSphere = false;
            // Mostrar ImoSphere apenas se não houver filtro de agência OU se o filtro for para a agência especial ImoSphere (id 0 ou nome "ImoSphere")
            if (!agencyId.HasValue || agencyId == 0)
            {
                showImoSphere = true;
            }
            else
            {
                var agency = await _context.Agencies.FindAsync(agencyId);
                if (agency != null && agency.Name == "ImoSphere")
                {
                    showImoSphere = true;
                }
            }
            // Adicionar SuperAdmins no topo (só se não há filtro por admin específico)
            if (showImoSphere && string.IsNullOrEmpty(adminId))
            {
                var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
                if (superAdmins.Any())
                {
                    var superAdminHierarchy = new UserHierarchyViewModel
                    {
                        AgencyId = 0, // ID especial para ImoSphere
                        AgencyName = "ImoSphere"
                    };
                    foreach (var superAdmin in superAdmins)
                    {
                        var adminGroup = new AdminGroup { Admin = superAdmin };
                        adminGroup.Comerciais = new List<ApplicationUser>(); // SuperAdmins não têm comerciais
                        superAdminHierarchy.Admins.Add(adminGroup);
                    }
                    hierarchy.Add(superAdminHierarchy);
                }
            }
            // Se há filtro por admin específico, buscar a agência desse admin
            if (!string.IsNullOrEmpty(adminId))
            {
                var adminAgencyUser = await _context.AgencyUsers
                    .Include(au => au.Agency)
                    .FirstOrDefaultAsync(au => au.UserId == adminId && au.Role == "Admin");
                if (adminAgencyUser?.Agency != null)
                {
                    var agencyHierarchy = new UserHierarchyViewModel
                    {
                        AgencyId = adminAgencyUser.Agency.Id,
                        AgencyName = adminAgencyUser.Agency.Name
                    };
                    // Buscar o admin específico
                    var admin = await _userManager.FindByIdAsync(adminId);
                    if (admin != null)
                    {
                        var adminGroup = new AdminGroup { Admin = admin };
                        // Buscar comerciais desse admin específico
                        var agencyUsers = await _context.AgencyUsers
                            .Include(au => au.User)
                            .Where(au => au.AgencyId == adminAgencyUser.Agency.Id)
                            .ToListAsync();
                        adminGroup.Comerciais = agencyUsers
                            .Where(au => au.Role == "Comercial" && au.AdminId == admin.Id)
                            .Select(au => au.User)
                            .ToList();
                        agencyHierarchy.Admins.Add(adminGroup);
                    }
                    hierarchy.Add(agencyHierarchy);
                }
            }
            else
            {
                // Lógica normal sem filtro por admin específico
                var agenciesQuery = _context.Agencies.AsQueryable();
                if (agencyId.HasValue && agencyId != 0)
                {
                    agenciesQuery = agenciesQuery.Where(a => a.Id == agencyId.Value);
                }
                agenciesQuery = agenciesQuery.Where(a => a.Name != "ImoSphere");
                var agencies = await agenciesQuery.ToListAsync();
                foreach (var agency in agencies)
                {
                    var agencyHierarchy = new UserHierarchyViewModel
                    {
                        AgencyId = agency.Id,
                        AgencyName = agency.Name
                    };
                    // Buscar todos os AgencyUsers da agência
                    var agencyUsers = await _context.AgencyUsers
                        .Include(au => au.User)
                        .Where(au => au.AgencyId == agency.Id)
                        .ToListAsync();
                    // Admins
                    var admins = agencyUsers.Where(au => au.Role == "Admin").Select(au => au.User).ToList();
                    // Comerciais sem admin
                    var comerciaisSemAdmin = agencyUsers.Where(au => au.Role == "Comercial" && au.AdminId == null).Select(au => au.User).ToList();
                    // Para cada admin, buscar comerciais desse admin
                    foreach (var admin in admins)
                    {
                        var adminGroup = new AdminGroup { Admin = admin };
                        adminGroup.Comerciais = agencyUsers
                            .Where(au => au.Role == "Comercial" && au.AdminId == admin.Id)
                            .Select(au => au.User)
                            .ToList();
                        agencyHierarchy.Admins.Add(adminGroup);
                    }
                    // Comerciais sem admin
                    agencyHierarchy.Comerciais = comerciaisSemAdmin;
                    hierarchy.Add(agencyHierarchy);
                }
            }
            return hierarchy;
        }
        
        private async Task<List<ApplicationUser>> GetAdminsForFilter(int? agencyId)
        {
            var query = _context.AgencyUsers
                .Where(au => au.Role == "Admin");
                
            if (agencyId.HasValue)
            {
                query = query.Where(au => au.AgencyId == agencyId.Value);
            }
            
            var adminUserIds = await query.Select(au => au.UserId).ToListAsync();
            return await _userManager.Users.Where(u => adminUserIds.Contains(u.Id)).ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> GetAdminsByAgency(int agencyId)
        {
            var adminUserIds = await _context.AgencyUsers
                .Where(au => au.AgencyId == agencyId && au.Role == "Admin")
                .Select(au => au.UserId)
                .ToListAsync();

            var admins = await _userManager.Users
                .Where(u => adminUserIds.Contains(u.Id))
                .Select(u => new { id = u.Id, userName = u.UserName })
                .ToListAsync();

            return Json(admins);
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
        public async Task<IActionResult> CreateUser(string email, string emailPrefix, string username, string role, string password, int? agencyId, string? adminId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(currentUser);
            var isSuperAdmin = roles.Contains("SuperAdmin");

            // Montar email corretamente
            if (string.IsNullOrEmpty(emailPrefix))
            {
                ModelState.AddModelError(string.Empty, "Email prefix is required.");
                if (isSuperAdmin) ViewBag.Agencies = _context.Agencies.ToList();
                return View();
            }

            if (role == "SuperAdmin")
            {
                email = emailPrefix + ".imosphere@imosphere.com";
                agencyId = null; // Não associar agência
            }
            else
            {
                string domain = "";
                if (isSuperAdmin)
                {
                    // Buscar agência pelo id
                    var agency = await _context.Agencies.FirstOrDefaultAsync(a => a.Id == agencyId);
                    if (agency == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agency is required.");
                        ViewBag.Agencies = _context.Agencies.ToList();
                        return View();
                    }
                    domain = $".{agency.Name.ToLower().Replace(" ", "")}@imosphere.com";
                }
                else
                {
                    // Buscar agência do admin
                    var agencyUser = await _context.AgencyUsers.Include(au => au.Agency).FirstOrDefaultAsync(au => au.UserId == currentUser.Id);
                    if (agencyUser?.Agency == null)
                        return Forbid();
                    domain = $".{agencyUser.Agency.Name.ToLower().Replace(" ", "")}@imosphere.com";
                    agencyId = agencyUser.AgencyId;
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
                            var agencyUser = new AgencyUser
                            {
                                UserId = user.Id,
                                AgencyId = agencyToAssign.Value,
                                Role = role
                            };
                            // Se for comercial, guardar AdminId
                            if (role == "Comercial")
                            {
                                if (isSuperAdmin && !string.IsNullOrEmpty(adminId))
                                {
                                    agencyUser.AdminId = adminId;
                                }
                                else if (!isSuperAdmin)
                                {
                                    // Admin comum: o próprio é o responsável
                                    agencyUser.AdminId = currentUser.Id;
                                }
                            }
                            _context.AgencyUsers.Add(agencyUser);
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

        [HttpGet]
        public async Task<IActionResult> CheckUserDependencies(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { canDelete = false, needsTransfer = false, message = "Utilizador não encontrado." });
            
            // PROTEÇÃO CRÍTICA: Nunca permitir eliminar o SuperAdmin principal
            if (user.Email == "imosphere.admin@imosphere.com")
                return Json(new { canDelete = false, needsTransfer = false, message = "Não é possível eliminar o SuperAdmin principal do sistema." });
            
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            if (role == "Comercial")
            {
                // Verificar se tem casas
                var agencyUser = await _context.AgencyUsers.FirstOrDefaultAsync(au => au.UserId == user.Id);
                var properties = await _context.Properties.Where(p => p.CreatedByUserId == user.Id).ToListAsync();
                if (properties.Count == 0)
                    return Json(new { canDelete = true, needsTransfer = false });
                // Procurar outros comerciais da mesma equipa
                var substitutes = await _context.AgencyUsers
                    .Where(au => au.AgencyId == agencyUser.AgencyId && au.Role == "Comercial" && au.UserId != user.Id)
                    .Select(au => new { id = au.UserId, name = au.User.UserName })
                    .ToListAsync();
                if (substitutes.Count == 0)
                    return Json(new { canDelete = false, needsTransfer = true, message = "Não é possível eliminar este comercial porque não existe comercial substituto na equipa.", substitutes = new object[0] });
                return Json(new { canDelete = false, needsTransfer = true, message = $"Este comercial tem {properties.Count} casa(s). Escolha um comercial substituto:", substitutes });
            }
            else if (role == "Admin")
            {
                // Verificar se tem comerciais
                var agencyUser = await _context.AgencyUsers.FirstOrDefaultAsync(au => au.UserId == user.Id);
                var comerciais = await _context.AgencyUsers.Where(au => au.AdminId == user.Id).ToListAsync();
                if (comerciais.Count == 0)
                    return Json(new { canDelete = true, needsTransfer = false });
                // Procurar outros admins da agência
                var substitutes = await _context.AgencyUsers
                    .Where(au => au.AgencyId == agencyUser.AgencyId && au.Role == "Admin" && au.UserId != user.Id)
                    .Select(au => new { id = au.UserId, name = au.User.UserName })
                    .ToListAsync();
                if (substitutes.Count == 0)
                    return Json(new { canDelete = false, needsTransfer = true, message = "Não é possível eliminar este admin porque não existe admin substituto na agência.", substitutes = new object[0] });
                return Json(new { canDelete = false, needsTransfer = true, message = $"Este admin tem {comerciais.Count} comercial(is). Escolha um admin substituto:", substitutes });
            }
            else
            {
                // SuperAdmin/User: eliminação direta
                return Json(new { canDelete = true, needsTransfer = false });
            }
        }

        [HttpPost]
        [Route("Admin/TransferAndDeleteUser")]
        public async Task<IActionResult> TransferAndDeleteUser(string id, string substituteId)
        {
            Console.WriteLine($"[TransferAndDeleteUser] id recebido: {id}, substituteId: {substituteId}");
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    Console.WriteLine($"[TransferAndDeleteUser] Utilizador não encontrado para id: {id}");
                    return Json(new { success = false, message = "Utilizador não encontrado." });
                }
                
                // PROTEÇÃO CRÍTICA: Nunca permitir eliminar o SuperAdmin principal
                if (user.Email == "imosphere.admin@imosphere.com")
                {
                    Console.WriteLine($"[TransferAndDeleteUser] Tentativa de eliminar SuperAdmin principal bloqueada: {user.Email}");
                    return Json(new { success = false, message = "Não é possível eliminar o SuperAdmin principal do sistema." });
                }
                
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();
                if (role == "Comercial")
                {
                    // Transferir casas
                    var properties = await _context.Properties.Where(p => p.CreatedByUserId == user.Id).ToListAsync();
                    foreach (var prop in properties)
                    {
                        prop.CreatedByUserId = substituteId;
                    }
                    // Transferir conversas de chat onde é Comercial
                    var convsComercial = await _context.ChatConversations.Where(c => c.ComercialId == user.Id).ToListAsync();
                    foreach (var conv in convsComercial)
                    {
                        conv.ComercialId = substituteId;
                    }
                    // Transferir conversas de chat onde é User
                    var convsUser = await _context.ChatConversations.Where(c => c.UserId == user.Id).ToListAsync();
                    foreach (var conv in convsUser)
                    {
                        conv.UserId = substituteId;
                    }
                    // Remover mensagens do utilizador
                    var messages = await _context.ChatMessages.Where(m => m.SenderId == user.Id).ToListAsync();
                    _context.ChatMessages.RemoveRange(messages);
                    await _context.SaveChangesAsync();
                    // Eliminar AgencyUser e ApplicationUser
                    var agencyUser = await _context.AgencyUsers.FirstOrDefaultAsync(au => au.UserId == user.Id);
                    if (agencyUser != null)
                        _context.AgencyUsers.Remove(agencyUser);
                    await _context.SaveChangesAsync();
                    await _userManager.DeleteAsync(user);
                    return Json(new { success = true });
                }
                else if (role == "Admin")
                {
                    // Transferir comerciais
                    var comerciais = await _context.AgencyUsers.Where(au => au.AdminId == user.Id).ToListAsync();
                    foreach (var comercial in comerciais)
                    {
                        comercial.AdminId = substituteId;
                    }
                    // Transferir conversas de chat onde é User ou Comercial
                    var convsComercial = await _context.ChatConversations.Where(c => c.ComercialId == user.Id).ToListAsync();
                    foreach (var conv in convsComercial)
                    {
                        conv.ComercialId = substituteId;
                    }
                    var convsUser = await _context.ChatConversations.Where(c => c.UserId == user.Id).ToListAsync();
                    foreach (var conv in convsUser)
                    {
                        conv.UserId = substituteId;
                    }
                    // Remover mensagens do utilizador
                    var messages = await _context.ChatMessages.Where(m => m.SenderId == user.Id).ToListAsync();
                    _context.ChatMessages.RemoveRange(messages);
                    await _context.SaveChangesAsync();
                    // Eliminar AgencyUser e ApplicationUser
                    var agencyUser = await _context.AgencyUsers.FirstOrDefaultAsync(au => au.UserId == user.Id);
                    if (agencyUser != null)
                        _context.AgencyUsers.Remove(agencyUser);
                    await _context.SaveChangesAsync();
                    await _userManager.DeleteAsync(user);
                    return Json(new { success = true });
                }
                else
                {
                    // SuperAdmin/User: eliminação direta
                    // Transferir conversas de chat onde é User ou Comercial
                    var convsComercial = await _context.ChatConversations.Where(c => c.ComercialId == user.Id).ToListAsync();
                    foreach (var conv in convsComercial)
                    {
                        conv.ComercialId = substituteId;
                    }
                    var convsUser = await _context.ChatConversations.Where(c => c.UserId == user.Id).ToListAsync();
                    foreach (var conv in convsUser)
                    {
                        conv.UserId = substituteId;
                    }
                    // Remover mensagens do utilizador
                    var messages = await _context.ChatMessages.Where(m => m.SenderId == user.Id).ToListAsync();
                    _context.ChatMessages.RemoveRange(messages);
                    await _context.SaveChangesAsync();
                    var agencyUser = await _context.AgencyUsers.FirstOrDefaultAsync(au => au.UserId == user.Id);
                    if (agencyUser != null)
                        _context.AgencyUsers.Remove(agencyUser);
                    await _context.SaveChangesAsync();
                    await _userManager.DeleteAsync(user);
                    return Json(new { success = true });
                }
            }
            catch (DbUpdateException ex)
            {
                // Diagnóstico: procurar dependências que ainda bloqueiam
                var dependencies = new List<string>();
                if (await _context.Properties.AnyAsync(p => p.CreatedByUserId == id))
                    dependencies.Add("propriedades");
                if (await _context.ChatConversations.AnyAsync(c => c.UserId == id || c.ComercialId == id))
                    dependencies.Add("conversas de chat");
                if (await _context.ChatMessages.AnyAsync(m => m.SenderId == id))
                    dependencies.Add("mensagens de chat");
                if (await _context.AgencyUsers.AnyAsync(au => au.UserId == id || au.AdminId == id))
                    dependencies.Add("registos de agência");
                var msg = dependencies.Count > 0
                    ? $"Não foi possível eliminar o utilizador porque ainda existem dependências: {string.Join(", ", dependencies)}. Transfira ou remova todas as referências antes de eliminar."
                    : "Erro ao eliminar utilizador. Por favor, tente novamente.";
                return Json(new { success = false, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro inesperado ao eliminar utilizador: " + ex.Message });
            }
        }
    }
}