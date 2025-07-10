using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImoSphere.Models
{
    public class Agency
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Property> Properties { get; set; }
        public ICollection<AgencyUser> AgencyUsers { get; set; }
    }
}