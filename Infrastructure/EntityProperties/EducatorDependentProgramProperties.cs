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
    internal class EducatorDependentProgramProperties
    {
        
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<EducatorDependentProgram> builder)
        {
            //_ = builder.ToTable("EducatorDependentPrograms");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<EducatorDependentProgram> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<EducatorDependentProgram> builder)
        {
            builder.HasOne(x => x.Educator).WithMany(x => x.EducatorDependentPrograms).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.DependentProgram).WithMany(x => x.EducatorDependentPrograms).HasForeignKey(x => x.DependentProgramId).OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<EducatorDependentProgram> builder)
        {
            // If you need it, use it here.
        }
    }
}
