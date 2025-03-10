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
    public static class UserRoleProgramProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<UserRoleProgram> builder)
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
        private static void SeedData(EntityTypeBuilder<UserRoleProgram> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<UserRoleProgram> builder)
        {
            builder.HasOne(x => x.UserRole).WithMany(x => x.UserRolePrograms).HasForeignKey(x => x.UserRoleId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Program).WithMany(x => x.UserRolePrograms).HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<UserRoleProgram> builder)
        {
            // If you need it, use it here.
        }
    }
}
