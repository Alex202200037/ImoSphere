using Microsoft.AspNetCore.SignalR;
using ImoSphere.Data;
using ImoSphere.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ImoSphere.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SendMessage(string conversationId, string message)
        {
            // Salvar mensagem no banco
            var conversation = await _context.ChatConversations
                .Include(c => c.Messages)
                .Include(c => c.Property)
                .FirstOrDefaultAsync(c => c.Id.ToString() == conversationId);
            
            if (conversation != null)
            {
                // Obter o utilizador autenticado
                var senderId = Context.UserIdentifier;
                var sender = await _context.Users.FindAsync(senderId);
                var senderName = sender?.UserName ?? senderId;

                // Determinar o role do sender na conversa
                string senderRole = "User";
                if (senderId == conversation.ComercialId)
                {
                    // Verificar se é admin ou comercial
                    var senderRoles = await _context.Roles.ToListAsync();
                    var userManager = (UserManager<ImoSphere.Models.ApplicationUser>)Context.GetHttpContext().RequestServices.GetService(typeof(UserManager<ImoSphere.Models.ApplicationUser>));
                    var senderUser = sender as ImoSphere.Models.ApplicationUser;
                    if (userManager != null && senderUser != null)
                    {
                        var roles = await userManager.GetRolesAsync(senderUser);
                        if (roles.Contains("Admin")) senderRole = "Admin";
                        else if (roles.Contains("Comercial")) senderRole = "Comercial";
                    }
                }
                else if (senderId == conversation.UserId)
                {
                    senderRole = "User";
                }

                var chatMessage = new ChatMessage
                {
                    ConversationId = conversation.Id,
                    SenderId = senderId,
                    Text = message,
                    SentAt = DateTime.UtcNow,
                    SenderName = senderName
                };
                
                _context.ChatMessages.Add(chatMessage);
                await _context.SaveChangesAsync();

                // Enviar para todos os clientes na conversa
                await Clients.Group(conversationId).SendAsync("ReceiveMessage", 
                    senderId, 
                    senderName,
                    senderRole,
                    message, 
                    chatMessage.SentAt.ToLocalTime().ToString("HH:mm"));

                // Enviar notificação para o destinatário
                var recipientId = senderId == conversation.UserId ? conversation.ComercialId : conversation.UserId;
                await SendNotificationToUser(recipientId, conversation.Property.Name);
            }
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        private async Task SendNotificationToUser(string userId, string propertyName)
        {
            // Contar mensagens não lidas
            var unreadCount = await _context.ChatMessages
                .Where(m => m.SenderId != userId && !m.IsRead && 
                           _context.ChatConversations.Any(c => c.Id == m.ConversationId && 
                                                              (c.UserId == userId || c.ComercialId == userId)))
                .CountAsync();

            // Enviar notificação para o grupo do utilizador
            await Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", 
                $"Nova mensagem sobre {propertyName}", unreadCount);
        }

        public override async Task OnConnectedAsync()
        {
            // Quando um utilizador se conecta, juntá-lo ao seu grupo pessoal
            if (Context.UserIdentifier != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{Context.UserIdentifier}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Quando um utilizador se desconecta, removê-lo do seu grupo pessoal
            if (Context.UserIdentifier != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{Context.UserIdentifier}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}