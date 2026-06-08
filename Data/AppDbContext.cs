using Microsoft.EntityFrameworkCore;
using TechMoveCRM.API.Models;

namespace TechMoveCRM.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent API — Client to Contracts (one-to-many)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Contracts)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Fluent API — Contract to ServiceRequests (one-to-many)
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Contract)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(sr => sr.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Store enum as string in database (more readable)
            modelBuilder.Entity<Contract>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.Status)
                .HasConversion<string>();
        }
    }
}