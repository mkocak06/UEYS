using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class StudentProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<Student> builder)
        {
            //_ = builder.ToTable("Students");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<Student> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<Student> builder)
        {
            builder.HasOne(x => x.User) .WithMany(x => x.Students) .HasForeignKey(x => x.UserId) .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Program) .WithMany(x => x.Students) .HasForeignKey(x => x.ProgramId) .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.OriginalProgram) .WithMany(x => x.OriginalStudents) .HasForeignKey(x => x.OriginalProgramId) .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.ProtocolProgram) .WithMany(x => x.ProtocolStudents) .HasForeignKey(x => x.ProtocolProgramId) .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Advisor) .WithMany(x => x.Students) .HasForeignKey(x => x.AdvisorId) .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Curriculum) .WithMany(x => x.Students) .HasForeignKey(x => x.CurriculumId) .OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<Student> builder)
        {
            // If you need it, use it here.
        }
    }
}
