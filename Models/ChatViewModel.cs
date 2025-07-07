using System.Collections.Generic;
using System;

namespace ImoSphere.Models
{
    public class ChatViewModel
    {
        public int ConversationId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyImage { get; set; }
        public string AgencyLogo { get; set; }
        public string ComercialName { get; set; }
        public string ComercialId { get; set; }
        public string ComercialAvatar { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string UserAvatar { get; set; }
        public bool CanSend { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSuperAdmin { get; set; }
        public List<ChatMessage> Messages { get; set; }
        public string CurrentUserId { get; set; }
        public Dictionary<string, string> SenderRoles { get; set; }
        public Property Property { get; set; }
        public string ComercialAdminName { get; set; }
    }

    public class ConversationListItemViewModel
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyImage { get; set; }
        public string AgencyLogo { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
} 