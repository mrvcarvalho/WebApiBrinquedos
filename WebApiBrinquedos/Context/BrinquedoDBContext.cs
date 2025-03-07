using Microsoft.EntityFrameworkCore;
using WebApiBrinquedos.Entity;

namespace WebApiBrinquedos.Context
{

    public class BrinquedoDbContext : DbContext
    {
        public BrinquedoDbContext(DbContextOptions<BrinquedoDbContext> options) : base(options) { }

        public DbSet<Brinquedo> Brinquedos { get; set; }

        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BrinquedoDbContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-2JVNN44;Initial Catalog=meulivrodereceitas;User ID=sa;Password=@Password123;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True");
        }
        */
    }
}
