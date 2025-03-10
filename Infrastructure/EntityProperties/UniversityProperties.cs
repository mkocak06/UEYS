using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class UniversityProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<University> builder)
        {
            //_ = builder.ToTable("Universities");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<University> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<University> builder)
        {
            builder.HasOne(x => x.Province) .WithMany(x => x.Universities) .HasForeignKey(x => x.ProvinceId) .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Institution) .WithMany(x => x.Universities) .HasForeignKey(x => x.InstitutionId) .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Manager) .WithMany(x => x.Universities) .HasForeignKey(x => x.ManagerId) .OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<University> builder)
        {
            // If you need it, use it here.
        }
    }
}
