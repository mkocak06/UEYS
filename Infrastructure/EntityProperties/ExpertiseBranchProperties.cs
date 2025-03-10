using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class ExpertiseBranchProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<ExpertiseBranch> builder)
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
        private static void SeedData(EntityTypeBuilder<ExpertiseBranch> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<ExpertiseBranch> builder)
        {
            builder.HasOne(x => x.Profession).WithMany(x => x.ExpertiseBranches).HasForeignKey(x => x.ProfessionId).OnDelete(DeleteBehavior.SetNull);
            builder.Property(x => x.Code).ValueGeneratedOnAdd();
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<ExpertiseBranch> builder)
        {
            // If you need it, use it here.
        }
    }
}
