using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class EducationTrackingProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<EducationTracking> builder)
        {
            //_ = builder.ToTable("EducationTrackings");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<EducationTracking> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<EducationTracking> builder)
        {
            builder.HasOne(x => x.Student).WithMany(x => x.EducationTrackings).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.ProcessOwner).WithMany(x => x.EducationTrackings).HasForeignKey(x => x.ProcessOwnerId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.FormerProgram).WithMany(x => x.EducationTrackingsFormer).HasForeignKey(x => x.FormerProgramId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Program).WithMany(x => x.EducationTrackings).HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.ThesisDefence).WithMany(x => x.EducationTrackings).HasForeignKey(x => x.ThesisDefenceId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<EducationTracking> builder)
        {
            // If you need it, use it here.
        }
    }
}
