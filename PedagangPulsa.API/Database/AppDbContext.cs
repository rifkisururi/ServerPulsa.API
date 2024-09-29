using Microsoft.EntityFrameworkCore;
using PedagangPulsa.API.Database.Entity;

namespace PedagangPulsa.API.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Produk> Produk  { get; set; }
        public DbSet<Transaksi> Transaksi { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Database/PedagangPulsa.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurasi kunci komposit untuk entitas Produk
            modelBuilder.Entity<Produk>()
                .HasKey(p => new { p.Vendor, p.Kode });  // Kunci komposit: Vendor + Kode
        }

    }
}
