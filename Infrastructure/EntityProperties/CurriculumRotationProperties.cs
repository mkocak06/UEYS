using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public class CurriculumRotationProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<CurriculumRotation> builder)
        {
            //_ = builder.ToTable("CurriculumRotations");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<CurriculumRotation> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<CurriculumRotation> builder)
        {
            builder.HasOne(x => x.Curriculum).WithMany(x => x.CurriculumRotations).HasForeignKey(x => x.CurriculumId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Rotation).WithMany(x => x.CurriculumRotations).HasForeignKey(x => x.RotationId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<CurriculumRotation> builder)
        {
            // If you need it, use it here.
        }
    }
}
