using System.Collections.Generic;

namespace ImoSphere.Models
{
    public class PropertyFilterViewModel
    {
        // Filtros
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinBathrooms { get; set; }
        public int? MaxBathrooms { get; set; }
        public int? MinBedrooms { get; set; }
        public int? MaxBedrooms { get; set; }
        public int? MinArea { get; set; }
        public int? MaxArea { get; set; }
        public int? MinYearBuilt { get; set; }
        public int? MaxYearBuilt { get; set; }
        public string Location { get; set; }
        public List<int> AgencyIds { get; set; } = new List<int>();

        // Ordenação
        public string SortBy { get; set; } = "Name"; // Default sort
        public string SortOrder { get; set; } = "asc"; // asc or desc

        // Resultados
        public IEnumerable<Property> Properties { get; set; } = new List<Property>();
        public IEnumerable<Agency> AvailableAgencies { get; set; } = new List<Agency>();

        // Contadores para mostrar quantos resultados
        public int TotalResults { get; set; }
        public int FilteredResults { get; set; }

        public bool ShowMap { get; set; } = false;
    }
}