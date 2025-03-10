using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityProperties
{
    public static class ProgramProperties
    {
        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static void OnModelCreating(EntityTypeBuilder<Program> builder)
        {
            //_ = builder.ToTable("Programs");
            //_ = builder.ThrowIfNull(nameof(builder));

            SetProperties(builder);
            SetForeignKeys(builder);
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SeedData(EntityTypeBuilder<Program> builder)
        {
            // If you need it, use it here.
        }

        /// <summary>
        /// Sets the foreign keys.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetForeignKeys(EntityTypeBuilder<Program> builder)
        {
            builder.HasOne(x => x.Faculty).WithMany(x => x.Programs).HasForeignKey(x => x.FacultyId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Hospital).WithMany(x => x.Programs).HasForeignKey(x => x.HospitalId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.ExpertiseBranch).WithMany(x => x.Programs).HasForeignKey(x => x.ExpertiseBranchId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Manager).WithMany(x => x.Programs).HasForeignKey(x => x.ManagerId).OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static void SetProperties(EntityTypeBuilder<Program> builder)
        {
            // If you need it, use it here.
        }
    }
}
