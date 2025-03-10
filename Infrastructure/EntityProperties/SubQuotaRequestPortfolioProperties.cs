using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class SubQuotaRequestPortfolioProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<SubQuotaRequestPortfolio> builder)
        {
            //_ = builder.ToTable("SubQuotaRequestPortfolios");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<SubQuotaRequestPortfolio> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<SubQuotaRequestPortfolio> builder)
        {
            builder.HasOne(x => x.SubQuotaRequest).WithMany(x => x.SubQuotaRequestPortfolios).HasForeignKey(x => x.SubQuotaRequestId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Portfolio).WithMany(x => x.SubQuotaRequestPortfolios).HasForeignKey(x => x.PortfolioId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<SubQuotaRequestPortfolio> builder)
        {
            // If you need it, use it here.
        }
    }
}
