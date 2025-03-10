using Core.KDSModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class ENabizPortfolioProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<ENabizPortfolio> builder)
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
        private static void SeedData(EntityTypeBuilder<ENabizPortfolio> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<ENabizPortfolio> builder)
        {
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<ENabizPortfolio> builder)
        {
            // If you need it, use it here.
            builder.ToTable("hekim_portfoy_bilgileri", schema: "kds_integration").HasKey(a => a.id);
        }
    }
}
