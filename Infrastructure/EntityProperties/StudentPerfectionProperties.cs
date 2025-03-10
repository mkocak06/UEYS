using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityProperties
{
    internal class StudentPerfectionProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<StudentPerfection> builder)
        {
            //_ = builder.ToTable("StudentRotations");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<StudentPerfection> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<StudentPerfection> builder)
        {
            builder.HasOne(x => x.Student).WithMany(x => x.StudentPerfections).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Perfection).WithMany(x => x.StudentPerfections).HasForeignKey(x => x.PerfectionId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Program).WithMany(x => x.StudentPerfections).HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Educator).WithMany(x => x.StudentPerfections).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<StudentPerfection> builder)
        {
            // If you need it, use it here.
        }
    }
}
