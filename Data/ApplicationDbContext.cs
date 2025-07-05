using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImoSphere.Models;

namespace ImoSphere.Data
{
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<ChatConversation> ChatConversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AgencyUser>()
                .HasOne(au => au.User)
                .WithMany()
                .HasForeignKey(au => au.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AgencyUser>()
                .HasOne(au => au.Agency)
                .WithMany(a => a.AgencyUsers)
                .HasForeignKey(au => au.AgencyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relação opcional: Comercial -> Admin
            builder.Entity<AgencyUser>()
                .HasOne(au => au.Admin)
                .WithMany()
                .HasForeignKey(au => au.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Property -> ApplicationUser
            builder.Entity<Property>()
                .HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ChatConversation -> ApplicationUser (User)
            builder.Entity<ChatConversation>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ChatConversation -> ApplicationUser (Comercial)
            builder.Entity<ChatConversation>()
                .HasOne(c => c.Comercial)
                .WithMany()
                .HasForeignKey(c => c.ComercialId)
                .OnDelete(DeleteBehavior.Restrict);

            // ChatMessage -> ApplicationUser (Sender)
            builder.Entity<ChatMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
