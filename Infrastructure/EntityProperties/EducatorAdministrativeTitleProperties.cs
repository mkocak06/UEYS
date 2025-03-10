using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class EducatorAdministrativeTitleProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<EducatorAdministrativeTitle> builder)
        {
            //_ = builder.ToTable("EducatorAdministrativeTitles");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<EducatorAdministrativeTitle> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<EducatorAdministrativeTitle> builder)
        {
            builder.HasOne(x => x.Educator).WithMany(x => x.EducatorAdministrativeTitles).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.AdministrativeTitle).WithMany(x => x.EducatorAdministrativeTitles).HasForeignKey(x => x.AdministrativeTitleId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<EducatorAdministrativeTitle> builder)
        {
            // If you need it, use it here.
        }
    }
}
