using Core.Entities.Koru;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityProperties.AuthorizationProperties
{
    public static class UserRoleHospitalProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<UserRoleHospital> builder)
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
        private static void SeedData(EntityTypeBuilder<UserRoleHospital> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<UserRoleHospital> builder)
        {
            builder.HasOne(x => x.UserRole).WithMany(x => x.UserRoleHospitals).HasForeignKey(x => x.UserRoleId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Hospital).WithMany(x => x.UserRoleHospitals).HasForeignKey(x => x.HospitalId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<UserRoleHospital> builder)
        {
            // If you need it, use it here.
        }
    }
}
