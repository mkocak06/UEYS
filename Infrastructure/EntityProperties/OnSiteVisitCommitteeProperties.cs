using Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityProperties
{
    public class OnSiteVisitCommitteeProperties
    {
         
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<OnSiteVisitCommittee> builder)
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
        private static void SeedData(EntityTypeBuilder<OnSiteVisitCommittee> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<OnSiteVisitCommittee> builder)
        {
            builder.HasOne(s => s.User)
         .WithMany(c => c.OnSiteVisitCommittees)
         .HasForeignKey(s => s.UserId)
         .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(s => s.Form)
         .WithMany(c => c.OnSiteVisitCommittees)
         .HasForeignKey(s => s.FormId)
         .OnDelete(DeleteBehavior.SetNull);

        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<OnSiteVisitCommittee> builder)
        {
            // If you need it, use it here.
        }
    }
}
 
