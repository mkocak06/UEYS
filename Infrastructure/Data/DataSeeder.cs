using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace Infrastructure.Data
{
    public class DataSeeder
    {
        public static async System.Threading.Tasks.Task SeedAsync(ApplicationDbContext applicationDbContext)
        {
            applicationDbContext.Database.EnsureCreated();

            var count = await applicationDbContext.Users.CountAsync(x => x.Email == "admin@saglik.gov.tr");
            if (count == 0)
            {
                var canc = new CancellationToken(false);

                AuthRepository authRepository = new AuthRepository(applicationDbContext);
                await authRepository.Create(canc, new Core.Entities.User()
                {
                    Email = "admin@saglik.gov.tr",
                    Name = "Admin"
                }, "admin");
                await applicationDbContext.SaveChangesAsync();
            }


        }
    }
}
