using Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class ProvinceProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<Province> builder)
        {
            //_ = builder.ToTable("Provinces");

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<Province> builder)
        {
            //_ = builder.ThrowIfNull(nameof(builder));

            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<Province> builder)
        {
            //_ = builder.ThrowIfNull(nameof(builder));

            // If you need it, use it here.
         //  builder.Property(x => x.Code).ValueGeneratedOnAdd();
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<Province> builder)
        {
            //_ = builder.ThrowIfNull(nameof(builder));

            // If you need it, use it here.
        }
    }
}
