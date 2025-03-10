using Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Koru;

namespace Infrastructure.EntityProperties.AuthorizationProperties
{
    public static class UserRoleFacultyProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<UserRoleFaculty> builder)
        {
            //_ = builder.ToTable("Universities");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<UserRoleFaculty> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<UserRoleFaculty> builder)
        {
            builder.HasOne(x => x.UserRole).WithMany(x => x.UserRoleFaculties).HasForeignKey(x => x.UserRoleId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Faculty).WithMany(x => x.UserRoleFaculties).HasForeignKey(x => x.FacultyId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.ExpertiseBranch).WithMany(x => x.UserRoleFaculties).HasForeignKey(x => x.ExpertiseBranchId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<UserRoleFaculty> builder)
        {
            // If you need it, use it here.
        }
    }
}
