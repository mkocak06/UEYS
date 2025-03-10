using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class AuthorizationDetailProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<AuthorizationDetail> builder)
        {
            //_ = builder.ToTable("AuthorisationDetails");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<AuthorizationDetail> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<AuthorizationDetail> builder)
        {
            builder.HasOne(x => x.Program).WithMany(x => x.AuthorizationDetails).HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.AuthorizationCategory).WithMany(x => x.AuthorizationDetails).HasForeignKey(x => x.AuthorizationCategoryId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<AuthorizationDetail> builder)
        {
            // If you need it, use it here.
        }
    }
}
