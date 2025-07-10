using Microsoft.AspNetCore.Identity;

namespace ImoSphere.Models
{
    public class AgencyUser
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int AgencyId { get; set; }
        public Agency Agency { get; set; }
        public string Role { get; set; } // Ex: Admin, Comercial

        // Hierarquia: só para comerciais
        public string? AdminId { get; set; } // UserId do admin responsável
        public ApplicationUser? Admin { get; set; }
    }
}