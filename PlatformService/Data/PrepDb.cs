using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext context)
        {
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding data...");
                context.Platforms.AddRange(
                        new Platform { Name = "DotNet", Publisher = "Microsoft", Cost = "Free" },
                        new Platform { Name = "SQL SERVER Express", Publisher = "Microsoft", Cost = "Free" },
                        new Platform { Name = "Kubernetes", Publisher = "Cloud Native CF", Cost = "Free" }
                    );
                context.SaveChanges();
            }
            else { Console.WriteLine("--> We already have data"); }
        }
    }
}
