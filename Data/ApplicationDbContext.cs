using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImoSphere.Models;

namespace ImoSphere.Data
{
    
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<AgencyUser> AgencyUsers { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
