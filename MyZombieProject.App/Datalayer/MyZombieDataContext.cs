using Microsoft.EntityFrameworkCore;
using MyZombieProject.App.Models;

namespace MyZombieProject.App.Datalayer
{
    public class MyZombieDataContext : DbContext
    {
        public DbSet<Shelter> Shelters => Set<Shelter>();

        public DbSet<Supply> Supplies => Set<Supply>();

        public DbSet<Survivor> Survivors => Set<Survivor>();

        public MyZombieDataContext(DbContextOptions<MyZombieDataContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=ZombieDb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Shelter>()
                .HasMany(s => s.Survivors)
                .WithOne(s => s.Shelter)
                .HasForeignKey(s => s.ShelterId)
                .IsRequired(false)          
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
