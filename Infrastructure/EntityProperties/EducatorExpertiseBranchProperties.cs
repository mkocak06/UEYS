using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class EducatorExpertiseBranchProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<EducatorExpertiseBranch> builder)
        {
            //_ = builder.ToTable("EducatorExpertiseBranches");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<EducatorExpertiseBranch> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<EducatorExpertiseBranch> builder)
        {
            builder.HasOne(x => x.ExpertiseBranch).WithMany(x => x.EducatorExpertiseBranches).HasForeignKey(x => x.ExpertiseBranchId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Educator).WithMany(x => x.EducatorExpertiseBranches).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<EducatorExpertiseBranch> builder)
        {
            // If you need it, use it here.
        }
    }
}
