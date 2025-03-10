using Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityProperties
{
    public class RelatedExpertiseBranchProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<RelatedExpertiseBranch> builder)
        {
            //_ = builder.ToTable("ExpertiseBranches");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<RelatedExpertiseBranch> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<RelatedExpertiseBranch> builder)
        {
            builder.HasKey(x => new { x.PrincipalBranchId, x.SubBranchId });
            builder.HasOne(x => x.SubBranch).WithMany(x => x.PrincipalBranches).HasForeignKey(x => x.SubBranchId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.PrincipalBranch).WithMany(x => x.SubBranches).HasForeignKey(x => x.PrincipalBranchId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<RelatedExpertiseBranch> builder)
        {
            // If you need it, use it here.
        }
    }
}
