using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class AdvisorThesisProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<AdvisorThesis> builder)
        {
            //_ = builder.ToTable("AdvisorTheses");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<AdvisorThesis> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<AdvisorThesis> builder)
        {
            builder.HasOne(x => x.Educator).WithMany(x => x.AdvisorTheses).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Thesis).WithMany(x => x.AdvisorTheses).HasForeignKey(x => x.ThesisId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.ExpertiseBranch).WithMany(x => x.AdvisorTheses).HasForeignKey(x => x.ExpertiseBranchId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.User).WithMany(x => x.AdvisorTheses).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<AdvisorThesis> builder)
        {
            // If you need it, use it here.
        }
    }
}
