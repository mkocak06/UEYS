using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class ExitExamProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<ExitExam> builder)
        {
            //_ = builder.ToTable("ExitExam");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<ExitExam> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<ExitExam> builder)
        {
            builder.HasOne(x => x.Student).WithMany(x => x.ExitExams).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Hospital).WithMany(x => x.ExitExams).HasForeignKey(x => x.HospitalId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.EducationTracking).WithMany(x => x.ExitExams).HasForeignKey(x => x.EducationTrackingId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Secretary).WithMany(x => x.ExitExams).HasForeignKey(x => x.SecretaryId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<ExitExam> builder)
        {
            // If you need it, use it here.
        }
    }
}
