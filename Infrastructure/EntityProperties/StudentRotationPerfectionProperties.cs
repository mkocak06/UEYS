using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public class StudentRotationPerfectionProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<StudentRotationPerfection> builder)
        {
            //_ = builder.ToTable("StudentRotationPerfections");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<StudentRotationPerfection> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<StudentRotationPerfection> builder)
        {
            builder.HasOne(x => x.StudentRotation).WithMany(x => x.StudentRotationPerfections).HasForeignKey(x => x.StudentRotationId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Perfection).WithMany(x => x.StudentRotationPerfections).HasForeignKey(x => x.PerfectionId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Educator).WithMany(x => x.StudentRotationPerfections).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<StudentRotationPerfection> builder)
        {
            // If you need it, use it here.
        }
    }
}
