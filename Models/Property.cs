using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ImoSphere.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }

        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Area { get; set; }

        public string Location { get; set; }
        public int YearBuilt { get; set; }

        public int AgencyId { get; set; }
        public Agency Agency { get; set; }
        public ICollection<PropertyImage> Images { get; set; }

        public string CreatedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }

        public Property()
        {
            Images = new List<PropertyImage>();
        }
    }
}
