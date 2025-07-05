using System.Collections.Generic;

namespace ImoSphere.Models
{
    public class UserHierarchyViewModel
    {
        public int AgencyId { get; set; }
        public string AgencyName { get; set; } = string.Empty;
        public List<AdminGroup> Admins { get; set; } = new List<AdminGroup>();
        public List<ApplicationUser> Comerciais { get; set; } = new List<ApplicationUser>();
    }

    public class AdminGroup
    {
        public ApplicationUser Admin { get; set; } = null!;
        public List<ApplicationUser> Comerciais { get; set; } = new List<ApplicationUser>();
    }

    public class UserFilterViewModel
    {
        public int? SelectedAgencyId { get; set; }
        public string? SelectedAdminId { get; set; }
        public List<Agency> Agencies { get; set; } = new List<Agency>();
        public List<ApplicationUser> Admins { get; set; } = new List<ApplicationUser>();
    }
} 