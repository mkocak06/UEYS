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
    public static class UserRoleStudentProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<UserRoleStudent> builder)
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
        private static void SeedData(EntityTypeBuilder<UserRoleStudent> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<UserRoleStudent> builder)
        {
            builder.HasOne(x => x.UserRole).WithMany(x => x.UserRoleStudents).HasForeignKey(x => x.UserRoleId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Student).WithMany(x => x.UserRoleStudents).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<UserRoleStudent> builder)
        {
            // If you need it, use it here.
        }
    }
}
