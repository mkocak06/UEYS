using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class HospitalProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<Hospital> builder)
        {
            //_ = builder.ToTable("Hospitals");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<Hospital> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<Hospital> builder)
        {
            builder.HasOne(x => x.Province).WithMany(x => x.Hospitals).HasForeignKey(x => x.ProvinceId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Institution).WithMany(x => x.Hospitals).HasForeignKey(x => x.InstitutionId).OnDelete(DeleteBehavior.SetNull);
            //builder.HasOne(x => x.Manager).WithMany(x => x.Hospitals).HasForeignKey(x => x.ManagerId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Faculty).WithMany(x => x.Hospitals).HasForeignKey(x => x.FacultyId).OnDelete(DeleteBehavior.SetNull);
            builder.Property(x => x.Code).ValueGeneratedOnAdd();
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<Hospital> builder)
        {
            // If you need it, use it here.
        }
    }
}
