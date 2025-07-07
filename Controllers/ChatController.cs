using ImoSphere.Data;
using ImoSphere.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ImoSphere.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? propertyId = null, string comercialId = null, int? conversationId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ChatError"] = "Utilizador não autenticado.";
                return RedirectToAction("Conversations");
            }
            var userId = user.Id;
            var roles = await _userManager.GetRolesAsync(user);
            var isComercial = roles.Contains("Comercial");
            var isAdmin = roles.Contains("Admin");
            var isSuperAdmin = roles.Contains("SuperAdmin");
            var agencyUser = _context.AgencyUsers.FirstOrDefault(au => au.UserId == userId);

            // Se vier por conversationId, fluxo normal
            if (conversationId.HasValue)
            {
                var conv = await _context.ChatConversations
                    .Include(c => c.Property).ThenInclude(p => p.Agency)
                    .Include(c => c.Property).ThenInclude(p => p.Images)
                    .Include(c => c.Messages)
                    .Include(c => c.User)
                    .Include(c => c.Comercial)
                    .FirstOrDefaultAsync(c => c.Id == conversationId.Value);
                if (conv == null)
                {
                    TempData["ChatError"] = "Conversa não encontrada.";
                    return RedirectToAction("Conversations");
                }
                var prop = conv.Property;
                if (prop == null)
                {
                    TempData["ChatError"] = "Propriedade associada à conversa não encontrada.";
                    return RedirectToAction("Conversations");
                }
                if (prop.Agency == null)
                {
                    TempData["ChatError"] = "Agência da propriedade não encontrada.";
                    return RedirectToAction("Conversations");
                }
                if (prop.Images == null)
                    prop.Images = new List<PropertyImage>();
                var com = conv.Comercial ?? await _context.Users.FindAsync(conv.ComercialId) as ApplicationUser;
                var userParticipant = conv.User ?? await _context.Users.FindAsync(conv.UserId) as ApplicationUser;
                // Buscar admin responsável pelo comercial
                var comercialAgencyUser = await _context.AgencyUsers.FirstOrDefaultAsync(au => au.UserId == com.Id && au.Role == "Comercial");
                string comercialAdminName = null;
                if (comercialAgencyUser?.AdminId != null)
                {
                    var adminUser = await _context.Users.FindAsync(comercialAgencyUser.AdminId);
                    comercialAdminName = adminUser?.UserName;
                }
                // Permissões
                bool isConvParticipant = (userId == conv.UserId) || (userId == conv.ComercialId);
                bool isConvAgencyAdmin = isAdmin && agencyUser != null && prop.AgencyId == agencyUser.AgencyId;
                if (!isConvParticipant && !isConvAgencyAdmin && !isSuperAdmin) return Forbid();
                // Permissões de envio
                var convCanSend = false;
                if (!isSuperAdmin)
                {
                    if (userId == conv.UserId || userId == conv.ComercialId)
                        convCanSend = true;
                    else if (isAdmin && agencyUser != null && prop.AgencyId == agencyUser.AgencyId)
                        convCanSend = true;
                }
                // Construir dicionário de roles dos participantes
                var senderRoles = new Dictionary<string, string>();
                if (com != null)
                {
                    var comRoles = await _userManager.GetRolesAsync(com);
                    senderRoles[com.Id] = comRoles.Contains("Admin") ? "Admin" : (comRoles.Contains("Comercial") ? "Comercial" : "Cliente");
                }
                if (userParticipant != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(userParticipant);
                    senderRoles[userParticipant.Id] = userRoles.Contains("Admin") ? "Admin" : (userRoles.Contains("Comercial") ? "Comercial" : "Cliente");
                }
                foreach (var m in conv.Messages)
                {
                    if (!senderRoles.ContainsKey(m.SenderId))
                    {
                        var userParticipantMsgObj = await _context.Users.FindAsync(m.SenderId) as ApplicationUser;
                        if (userParticipantMsgObj != null)
                        {
                            var rolesSender = await _userManager.GetRolesAsync(userParticipantMsgObj);
                            senderRoles[userParticipantMsgObj.Id] = rolesSender.Contains("Admin") ? "Admin" : (rolesSender.Contains("Comercial") ? "Comercial" : "Cliente");
                        }
                        else
                        {
                            senderRoles[m.SenderId] = "Cliente";
                        }
                    }
                }
                var convVm = new ChatViewModel
                {
                    ConversationId = conv.Id,
                    PropertyId = prop.Id,
                    PropertyName = prop.Name ?? "Propriedade",
                    PropertyImage = prop.Images.FirstOrDefault()?.ImageUrl ?? "/images/placeholder.png",
                    AgencyLogo = $"/images/{prop.Agency?.Name?.ToLower()}.png",
                    ComercialName = com?.UserName ?? "Comercial",
                    ComercialId = com?.Id,
                    ComercialAvatar = null,
                    ComercialAdminName = comercialAdminName,
                    UserName = userParticipant?.UserName ?? "Utilizador",
                    UserId = userParticipant?.Id ?? "",
                    UserAvatar = null,
                    CanSend = convCanSend,
                    IsAdmin = isAdmin,
                    IsSuperAdmin = isSuperAdmin,
                    Messages = conv.Messages?.OrderBy(m => m.SentAt).ToList() ?? new List<ChatMessage>(),
                    CurrentUserId = userId,
                    SenderRoles = senderRoles,
                    Property = prop
                };
                return View(convVm);
            }

            // Se não vier conversationId, propertyId e comercialId são obrigatórios
            if (!propertyId.HasValue || string.IsNullOrEmpty(comercialId))
            {
                TempData["ChatError"] = "Dados insuficientes para abrir o chat.";
                return RedirectToAction("Conversations");
            }
            var property = await _context.Properties.Include(p => p.Agency).Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == propertyId);
            if (property == null)
            {
                TempData["ChatError"] = "Propriedade não encontrada.";
                return RedirectToAction("Conversations");
            }
            // Descobrir comercial responsável
            var comercialUserId = comercialId ?? property.CreatedByUserId;
            var comercial = await _context.Users.FindAsync(comercialUserId);
            if (comercial == null)
            {
                TempData["ChatError"] = "Comercial não encontrado.";
                return RedirectToAction("Conversations");
            }

            // Procurar conversa existente - pode ser como user ou como comercial
            var conversation = await _context.ChatConversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.PropertyId == propertyId && 
                                         ((c.UserId == userId && c.ComercialId == comercialUserId) || 
                                          (c.ComercialId == userId && c.UserId == comercialUserId)));
            // Se não existir conversa, mostrar view para iniciar contacto
            if (conversation == null)
            {
                // Buscar admin responsável pelo comercial
                var comercialAgencyUser = await _context.AgencyUsers.FirstOrDefaultAsync(au => au.UserId == comercial.Id && au.Role == "Comercial");
                string comercialAdminName = null;
                if (comercialAgencyUser?.AdminId != null)
                {
                    var adminUser = await _context.Users.FindAsync(comercialAgencyUser.AdminId);
                    comercialAdminName = adminUser?.UserName;
                }
                // Buscar interessado
                var userParticipant = await _context.Users.FindAsync(userId);
                var chatViewModel = new ChatViewModel
                {
                    ConversationId = 0,
                    PropertyId = property.Id,
                    PropertyName = property.Name,
                    PropertyImage = property.Images.FirstOrDefault()?.ImageUrl ?? "/images/placeholder.png",
                    AgencyLogo = $"/images/{property.Agency?.Name?.ToLower()}.png",
                    ComercialName = comercial.UserName,
                    ComercialId = comercial.Id,
                    ComercialAvatar = null,
                    ComercialAdminName = comercialAdminName,
                    UserName = userParticipant?.UserName ?? "Utilizador",
                    UserId = userParticipant?.Id ?? "",
                    UserAvatar = null,
                    CanSend = !isSuperAdmin && !isAdmin,
                    IsAdmin = isAdmin,
                    IsSuperAdmin = isSuperAdmin,
                    Messages = new List<ChatMessage>(),
                    CurrentUserId = userId,
                    Property = property
                };
                return View("Index", chatViewModel);
            }
            // Se a conversa existe, mas o utilizador não é participante nem admin da agência, não mostrar o chat
            bool isParticipant = (userId == conversation.UserId) || (userId == conversation.ComercialId);
            bool isAgencyAdmin = isAdmin && agencyUser != null && property.AgencyId == agencyUser.AgencyId;
            if (!isParticipant && !isAgencyAdmin)
            {
                return Forbid();
            }

            // Permissões
            var canSend = false;
            if (!isSuperAdmin)
            {
                if (userId == conversation.UserId || userId == conversation.ComercialId)
                {
                    canSend = true;
                }
                else if (isAdmin && agencyUser != null && property.AgencyId == agencyUser.AgencyId)
                {
                    canSend = true;
                }
            }
            // Identificar "outro" participante
            string otherParticipantName = null;
            string otherParticipantId = null;
            if (userId == conversation.UserId)
            {
                otherParticipantName = comercial.UserName;
                otherParticipantId = comercial.Id;
            }
            else
            {
                var userParticipantObj = await _context.Users.FindAsync(conversation.UserId) as ApplicationUser;
                otherParticipantName = userParticipantObj?.UserName;
                otherParticipantId = userParticipantObj?.Id;
            }
            // ViewModel
            var comercialAgencyUser2 = await _context.AgencyUsers.FirstOrDefaultAsync(au => au.UserId == comercial.Id && au.Role == "Comercial");
            string comercialAdminName2 = null;
            if (comercialAgencyUser2?.AdminId != null)
            {
                var adminUser2 = await _context.Users.FindAsync(comercialAgencyUser2.AdminId);
                comercialAdminName2 = adminUser2?.UserName;
            }
            var vm = new ChatViewModel
            {
                ConversationId = conversation.Id,
                PropertyId = property.Id,
                PropertyName = property.Name,
                PropertyImage = property.Images.FirstOrDefault()?.ImageUrl ?? "/images/placeholder.png",
                AgencyLogo = $"/images/{property.Agency?.Name?.ToLower()}.png",
                ComercialName = comercial.UserName,
                ComercialId = comercial.Id,
                ComercialAvatar = null,
                ComercialAdminName = comercialAdminName2,
                UserName = otherParticipantName,
                UserId = otherParticipantId,
                UserAvatar = null,
                CanSend = canSend,
                IsAdmin = isAdmin,
                IsSuperAdmin = isSuperAdmin,
                Messages = conversation.Messages.OrderBy(m => m.SentAt).ToList(),
                CurrentUserId = userId,
                Property = property
            };
            ViewBag.OtherParticipantName = otherParticipantName;
            ViewBag.OtherParticipantId = otherParticipantId;
            // Antes de passar as mensagens para o ViewModel, preencher SenderName
            foreach (var m in conversation.Messages)
            {
                if (string.IsNullOrEmpty(m.SenderName))
                {
                    var sender = await _context.Users.FindAsync(m.SenderId) as ApplicationUser;
                    m.SenderName = sender?.UserName ?? m.SenderId;
                }
            }
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            var roles = await _userManager.GetRolesAsync(user);
            var isComercial = roles.Contains("Comercial");
            var isAdmin = roles.Contains("Admin");
            var isSuperAdmin = roles.Contains("SuperAdmin");
            var agencyUser = _context.AgencyUsers.FirstOrDefault(au => au.UserId == userId);

            var conversation = await _context.ChatConversations
                .Include(c => c.Property)
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId);
            if (conversation == null)
            {
                Console.WriteLine($"[MarkAsRead] Conversa não encontrada: {request.ConversationId}");
                return Json(new { success = false, error = "Conversa não encontrada." });
            }
            var prop = conversation.Property;
            bool isParticipant = (userId == conversation.UserId) || (userId == conversation.ComercialId);
            bool isAgencyAdmin = isAdmin && agencyUser != null && prop != null && prop.AgencyId == agencyUser.AgencyId;
            if (!isParticipant && !isAgencyAdmin && !isSuperAdmin)
            {
                Console.WriteLine($"[MarkAsRead] Utilizador {userId} não tem permissão para marcar como lida a conversa {request.ConversationId}");
                return Json(new { success = false, error = "Sem permissão." });
            }
            var messages = await _context.ChatMessages
                .Where(m => m.ConversationId == request.ConversationId && m.SenderId != userId && !m.IsRead)
                .ToListAsync();
            Console.WriteLine($"[MarkAsRead] Utilizador {userId} vai marcar {messages.Count} mensagens como lidas na conversa {request.ConversationId}");
            foreach (var message in messages)
            {
                message.IsRead = true;
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, marked = messages.Count });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartContactSend(int propertyId, string comercialId, string message)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ChatError"] = "Utilizador não autenticado.";
                return RedirectToAction("Conversations");
            }
            var userId = user.Id;
            var property = await _context.Properties.FindAsync(propertyId);
            if (property == null)
            {
                TempData["ChatError"] = "Propriedade não encontrada.";
                return RedirectToAction("Conversations");
            }
            var comercial = await _context.Users.FindAsync(comercialId);
            if (comercial == null)
            {
                TempData["ChatError"] = "Comercial não encontrado.";
                return RedirectToAction("Conversations");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["ChatError"] = "A mensagem não pode ser vazia.";
                return RedirectToAction("Index", new { propertyId = propertyId, comercialId = comercialId });
            }
            Console.WriteLine($"[CHAT] StartContactSend: userId={userId}, propertyId={propertyId}, comercialId={comercialId}, message={message}");
            // Verificar se já existe conversa
            var conversation = await _context.ChatConversations
                .FirstOrDefaultAsync(c => c.PropertyId == propertyId &&
                    ((c.UserId == userId && c.ComercialId == comercialId) ||
                     (c.ComercialId == userId && c.UserId == comercialId)));
            if (conversation == null)
            {
                conversation = new ChatConversation
                {
                    PropertyId = propertyId,
                    UserId = userId,
                    ComercialId = comercialId,
                    Messages = new List<ChatMessage>()
                };
                _context.ChatConversations.Add(conversation);
                await _context.SaveChangesAsync();
                Console.WriteLine($"[CHAT] Nova conversa criada: conversationId={conversation.Id}");
            }
            else
            {
                Console.WriteLine($"[CHAT] Conversa já existia: conversationId={conversation.Id}");
            }
            // Adicionar a primeira mensagem
            var chatMessage = new ChatMessage
            {
                ConversationId = conversation.Id,
                SenderId = userId,
                Text = message,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                SenderName = user.UserName
            };
            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();
            Console.WriteLine($"[CHAT] Mensagem criada: messageId={chatMessage.Id}, conversationId={conversation.Id}");
            // Redirecionar para o chat
            return RedirectToAction("Index", new { propertyId = propertyId, comercialId = comercialId });
        }

        public async Task<IActionResult> Conversations()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            var roles = await _userManager.GetRolesAsync(user);
            var isComercial = roles.Contains("Comercial");
            var isAdmin = roles.Contains("Admin");
            var isSuperAdmin = roles.Contains("SuperAdmin");
            var agencyUser = _context.AgencyUsers.FirstOrDefault(au => au.UserId == userId);

            var userIdStr = userId ?? string.Empty;
            Console.WriteLine($"[CHAT] Listando conversas para: userId={userIdStr}, roles={string.Join(",", roles)}");
            List<ChatConversation> conversations = new List<ChatConversation>();
            if (isSuperAdmin)
            {
                conversations = _context.ChatConversations
                    .Include(c => c.Property)
                    .Include(c => c.Property.Images)
                    .Include(c => c.Property.Agency)
                    .Include(c => c.Messages)
                    .Where(c => c.Messages.Any())
                    .ToList();
                Console.WriteLine($"[CHAT] SuperAdmin: encontradas {conversations.Count} conversas");
            }
            else if (isAdmin && agencyUser != null)
            {
                conversations = _context.ChatConversations
                    .Include(c => c.Property)
                    .Include(c => c.Property.Images)
                    .Include(c => c.Property.Agency)
                    .Include(c => c.Messages)
                    .Where(c => c.Property.AgencyId == agencyUser.AgencyId && c.Messages.Any())
                    .ToList();
                Console.WriteLine($"[CHAT] Admin: encontradas {conversations.Count} conversas para agencyId={agencyUser.AgencyId}");
            }
            else if (isComercial)
            {
                conversations = _context.ChatConversations
                    .Include(c => c.Property)
                    .Include(c => c.Property.Images)
                    .Include(c => c.Property.Agency)
                    .Include(c => c.Messages)
                    .Where(c => c.ComercialId == userIdStr && c.Messages.Any())
                    .ToList();
                Console.WriteLine($"[CHAT] Comercial: encontradas {conversations.Count} conversas para comercialId={userIdStr}");
            }
            else
            {
                var allUserConversations = _context.ChatConversations
                    .Include(c => c.Property)
                    .Include(c => c.Property.Images)
                    .Include(c => c.Property.Agency)
                    .Include(c => c.Messages)
                    .Where(c => c.UserId == userIdStr)
                    .ToList();
                Console.WriteLine($"[CHAT] User: todas as conversas para userId={userIdStr}: {string.Join(", ", allUserConversations.Select(c => $"ConvId={c.Id}, UserId={c.UserId}, ComercialId={c.ComercialId}"))}");
                conversations = allUserConversations.Where(c => c.Messages.Any()).ToList();
                Console.WriteLine($"[CHAT] User: encontradas {conversations.Count} conversas com mensagens para userId={userIdStr}");
            }
            conversations = conversations.OrderByDescending(c =>
                c.Messages.Any() ? c.Messages.Max(m => m.SentAt) : DateTime.MinValue).ToList();
            var conversationList = conversations.Select(c => new ConversationListItemViewModel
            {
                Id = c.Id,
                PropertyId = c.PropertyId,
                PropertyName = c.Property?.Name ?? "Propriedade",
                PropertyImage = c.Property?.Images?.FirstOrDefault()?.ImageUrl ?? "/images/placeholder.png",
                AgencyLogo = c.Property?.Agency != null ? $"/images/{c.Property.Agency.Name?.ToLower()}.png" : null,
                LastMessage = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Text ?? "Nenhuma mensagem",
                LastMessageTime = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.SentAt,
                UnreadCount = c.Messages.Count(m => m.SenderId != userIdStr && !m.IsRead)
            }).ToList();
            ViewBag.CurrentUserId = userIdStr;
            if (conversationList == null)
                conversationList = new List<ConversationListItemViewModel>();
            return View("Conversations", conversationList);
        }
    }
}