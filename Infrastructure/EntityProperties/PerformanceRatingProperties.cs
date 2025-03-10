using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class PerformanceRatingProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<PerformanceRating> builder)
        {
            //_ = builder.ToTable("PerformanceRatings");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<PerformanceRating> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<PerformanceRating> builder)
        {
            builder.HasOne(x => x.Student).WithMany(x => x.PerformanceRatings).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Educator).WithMany(x => x.PerformanceRatings).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<PerformanceRating> builder)
        {
            // If you need it, use it here.
        }
    }
}
