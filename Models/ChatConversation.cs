using System;
using System.Collections.Generic;

namespace ImoSphere.Models
{
    public class ChatConversation
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; }
        public string UserId { get; set; } // Interessado
        public ApplicationUser User { get; set; }
        public string ComercialId { get; set; } // Responsável
        public ApplicationUser Comercial { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ChatMessage> Messages { get; set; }
    }
} 