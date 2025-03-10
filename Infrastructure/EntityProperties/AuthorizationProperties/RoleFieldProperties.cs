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
    public static class RoleFieldProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<RoleField> builder)
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
        private static void SeedData(EntityTypeBuilder<RoleField> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<RoleField> builder)
        {
            builder.HasOne(x => x.Field).WithMany(x => x.RoleFields).HasForeignKey(x => x.FieldId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Role).WithMany(x => x.RoleFields).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<RoleField> builder)
        {
            // If you need it, use it here.
        }
    }
}
