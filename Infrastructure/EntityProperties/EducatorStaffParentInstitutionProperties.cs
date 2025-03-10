using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class EducatorStaffParentInstitutionProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<EducatorStaffParentInstitution> builder)
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
        private static void SeedData(EntityTypeBuilder<EducatorStaffParentInstitution> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<EducatorStaffParentInstitution> builder)
        {
            builder.HasOne(x => x.Educator).WithMany(x => x.StaffParentInstitutions).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.StaffParentInstitution).WithMany(x => x.StaffEducators).HasForeignKey(x => x.StaffParentInstitutionId).OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<EducatorStaffParentInstitution> builder)
        {
            // If you need it, use it here.
        }
    }
}
