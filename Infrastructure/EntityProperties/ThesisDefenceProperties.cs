using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class ThesisDefenceProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<ThesisDefence> builder)
        {
            //_ = builder.ToTable("ThesisDefences");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<ThesisDefence> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<ThesisDefence> builder)
        {
            builder.HasOne(x => x.Thesis).WithMany(x => x.ThesisDefences).HasForeignKey(x => x.ThesisId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Hospital).WithMany(x => x.ThesisDefences).HasForeignKey(x => x.HospitalId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<ThesisDefence> builder)
        {
            // If you need it, use it here.
        }
    }
}
