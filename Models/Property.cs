using System.ComponentModel.DataAnnotations;

namespace ImoSphere.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Area { get; set; }

        public string Location { get; set; }
        public int YearBuilt { get; set; }
    }
}
