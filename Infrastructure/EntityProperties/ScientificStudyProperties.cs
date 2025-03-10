using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class ScientificStudyProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<ScientificStudy> builder)
        {
            //_ = builder.ToTable("AuthorisationDetails");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<ScientificStudy> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<ScientificStudy> builder)
        {
            builder.HasOne(x => x.Student).WithMany(x => x.ScientificStudies).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Study).WithMany(x => x.ScientificStudies).HasForeignKey(x => x.StudyId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<ScientificStudy> builder)
        {
            // If you need it, use it here.
        }
    }
}
