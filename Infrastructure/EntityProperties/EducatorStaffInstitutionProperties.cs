using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class EducatorStaffInstitutionProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<EducatorStaffInstitution> builder)
        {
            //_ = builder.ToTable("EducatorStaffInstitutions");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<EducatorStaffInstitution> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<EducatorStaffInstitution> builder)
        {
            builder.HasOne(x => x.Educator).WithMany(x => x.StaffInstitutions).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.StaffInstitution).WithMany(x => x.EducatorStaffInstitutions).HasForeignKey(x => x.StaffInstitutionId).OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<EducatorStaffInstitution> builder)
        {
            // If you need it, use it here.
        }
    }
}
