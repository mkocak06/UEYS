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
    internal class JuryProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<Jury> builder)
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
        private static void SeedData(EntityTypeBuilder<Jury> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<Jury> builder)
        {
            builder.HasOne(x => x.Educator).WithMany(x => x.Juries).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.ThesisDefence).WithMany(x => x.Juries).HasForeignKey(x => x.ThesisDefenceId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.ExitExam).WithMany(x => x.Juries).HasForeignKey(x => x.ExitExamId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.User).WithMany(x => x.Juries).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<Jury> builder)
        {
            // If you need it, use it here.
        }
    }
}
