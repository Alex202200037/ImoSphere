using System;

namespace ImoSphere.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public ChatConversation Conversation { get; set; }
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public string SenderName { get; set; }
    }
}