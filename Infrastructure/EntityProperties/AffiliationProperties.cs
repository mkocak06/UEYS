using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class AffiliationProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<Affiliation> builder)
        {
            //_ = builder.ToTable("Affiliations");
            //_ = builder.ThrowIfNull(nameof(builder));
            
            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<Affiliation> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<Affiliation> builder)
        {
            builder.HasOne(x => x.Hospital).WithMany(x => x.Affiliations).HasForeignKey(x => x.HospitalId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Faculty).WithMany(x => x.Affiliations).HasForeignKey(x => x.FacultyId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<Affiliation> builder)
        {
            // If you need it, use it here.
        }
    }
}
