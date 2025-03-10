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
    public static class DependentProgramProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<DependentProgram> builder)
        {
            //_ = builder.ToTable("DependentPrograms");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<DependentProgram> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<DependentProgram> builder)
        {
            builder.HasOne(x => x.RelatedDependentProgram).WithMany(x => x.ChildPrograms).HasForeignKey(x => x.RelatedDependentProgramId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Program).WithMany(x => x.DependentPrograms).HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<DependentProgram> builder)
        {
            // If you need it, use it here.
        }
    }
}
