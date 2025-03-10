using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class OpinionFormProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<OpinionForm> builder)
        {
            //_ = builder.ToTable("OpinionForms");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<OpinionForm> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<OpinionForm> builder)
        {
            builder.HasOne(x => x.Student).WithMany(x => x.OpinionForms).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Educator).WithMany(x => x.EducatorOpinionForms).HasForeignKey(x => x.EducatorId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Secretary).WithMany(x => x.OpinionForms).HasForeignKey(x => x.SecretaryId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.ProgramManager).WithMany(x => x.ProgramManagerOpinionForms).HasForeignKey(x => x.ProgramManagerId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Program).WithMany(x => x.OpinionForms).HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<OpinionForm> builder)
        {
            // If you need it, use it here.
        }
    }
}
