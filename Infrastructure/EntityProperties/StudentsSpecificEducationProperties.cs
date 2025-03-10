using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class StudentsSpecificEducationProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<StudentsSpecificEducation> builder)
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
        private static void SeedData(EntityTypeBuilder<StudentsSpecificEducation> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<StudentsSpecificEducation> builder)
        {
            builder.HasOne(s => s.SpecificEducation)
            .WithMany(c => c.StudentsSpecificEducations)
            .HasForeignKey(s => s.SpesificEducationId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(s => s.Student)
            .WithMany(c => c.StudentsSpecificEducations)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(s => s.SpecificEducationPlace)
             .WithMany(c => c.StudentsSpecificEducations)
            .HasForeignKey(s => s.SpecificEducationPlaceId)
            .OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<StudentsSpecificEducation> builder)
        {
            // If you need it, use it here.
        }
    }
}
