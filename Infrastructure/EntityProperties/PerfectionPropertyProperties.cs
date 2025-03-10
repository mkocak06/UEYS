using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Infrastructure.EntityProperties
{
    public class PerfectionPropertyProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<PerfectionProperty> builder)
        {
            //_ = builder.ToTable("Educators");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<PerfectionProperty> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<PerfectionProperty> builder)
        {
            builder.HasOne(x => x.Property).WithMany(x => x.PerfectionProperties).HasForeignKey(x => x.PropertyId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Perfection).WithMany(x => x.PerfectionProperties).HasForeignKey(x => x.PerfectionId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<PerfectionProperty> builder)
        {
            // If you need it, use it here.
        }
    }
}
