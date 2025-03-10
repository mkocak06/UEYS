using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityProperties
{
    public static class FormStandardProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<FormStandard> builder)
        {
            //_ = builder.ToTable("Users");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<FormStandard> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<FormStandard> builder)
        {
            builder.HasOne(s => s.Standard)
            .WithMany(c => c.FormStandards)
            .HasForeignKey(s => s.StandardId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(s => s.Form)
             .WithMany(c => c.FormStandards)
              .HasForeignKey(s => s.FormId)
                .OnDelete(DeleteBehavior.SetNull);

        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<FormStandard> builder)
        {
            // If you need it, use it here.
        }
    }
}

