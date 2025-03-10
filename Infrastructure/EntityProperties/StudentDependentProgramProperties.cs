using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class StudentDependentProgramProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<StudentDependentProgram> builder)
        {
            //_ = builder.ToTable("StudentDependentPrograms");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<StudentDependentProgram> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<StudentDependentProgram> builder)
        {
            builder.HasOne(x => x.Student).WithMany(x => x.StudentDependentPrograms).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.DependentProgram).WithMany(x => x.StudentDependentPrograms).HasForeignKey(x => x.DependentProgramId).OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<StudentDependentProgram> builder)
        {
            // If you need it, use it here.
        }
    }
}
