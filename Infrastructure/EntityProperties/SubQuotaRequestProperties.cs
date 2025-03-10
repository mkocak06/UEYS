using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public class SubQuotaRequestProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<SubQuotaRequest> builder)
        {
            //_ = builder.ToTable("SubQuotaRequests");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<SubQuotaRequest> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<SubQuotaRequest> builder)
        {
            builder.HasOne(x => x.Program).WithMany(x => x.SubQuotaRequests).HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.QuotaRequest).WithMany(x => x.SubQuotaRequests).HasForeignKey(x => x.QuotaRequestId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<SubQuotaRequest> builder)
        {
            // If you need it, use it here.
        }
    }
}
