namespace TestApp
{
    using DataLayer;
    using DataLayer.Data.Seeding;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public static class SeedingExtension
    {
        public static IWebHost SeedData(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<ApplicationDbContext>();

                context.Users.RemoveRange(context.Users);
                context.Projects.RemoveRange(context.Projects);
                context.TimeLogs.RemoveRange(context.TimeLogs);
                context.SaveChanges();
                var seedingObject = new Seeding(context);
                seedingObject.SeedProjects();
                seedingObject.SeedUsers();
                seedingObject.SeedTimeLogs();
            }
            return host;
        }
    }
}
