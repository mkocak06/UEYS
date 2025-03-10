using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class ProgressReportProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<ProgressReport> builder)
        {
            //_ = builder.ToTable("ProgressReports");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<ProgressReport> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<ProgressReport> builder)
        {
            builder.HasOne(x => x.Thesis).WithMany(x => x.ProgressReports).HasForeignKey(x => x.ThesisId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Educator).WithMany(x => x.ProgressReports).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<ProgressReport> builder)
        {
            // If you need it, use it here.
        }
    }
}
