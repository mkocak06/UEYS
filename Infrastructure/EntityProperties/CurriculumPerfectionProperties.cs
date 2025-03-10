using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public class CurriculumPerfectionProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<CurriculumPerfection> builder)
        {
            //_ = builder.ToTable("CurriculumPerfections");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<CurriculumPerfection> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<CurriculumPerfection> builder)
        {
            builder.HasOne(x => x.Curriculum).WithMany(x => x.CurriculumPerfections).HasForeignKey(x => x.CurriculumId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Perfection).WithMany(x => x.CurriculumPerfections).HasForeignKey(x => x.PerfectionId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<CurriculumPerfection> builder)
        {
            // If you need it, use it here.
        }
    }
}
