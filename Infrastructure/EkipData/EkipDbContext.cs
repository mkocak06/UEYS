using Core.EkipModels;
using Core.Entities;
using Infrastructure.EntityProperties;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EkipData
{
    public class EkipDbContext : DbContext
    {
        public EkipDbContext(DbContextOptions<EkipDbContext> options) : base(options)
        {
        }

        #region DbSet
        public DbSet<Personel> Personeller { get; set; }
        public DbSet<Birim> Birimler { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Personel>().ToView("mv_personel", "integration").HasKey(x => x.mv_id);
            modelBuilder.Entity<PersonelHareketi>().ToView("mv_personel_hareketleri", "integration").HasKey(x => x.mv_id);
            modelBuilder.Entity<Birim>().ToView("mv_birim", "integration").HasKey(x => x.mv_id);

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
