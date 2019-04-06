namespace CrossRoadsStoreApp.Migrations
{
    using CrossRoadsStoreApp.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CrossRoadsStoreApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "CrossRoadsStoreApp.Models.ApplicationDbContext";
        }

        protected override void Seed(CrossRoadsStoreApp.Models.ApplicationDbContext context)
        {
            //Initializes app on seed with an admin user account.
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            if (!context.Users.Any(t => t.UserName == "admin@crossroads.ca"))
            {
                var user = new ApplicationUser { UserName = "admin@crossroads.ca", Email = "admin@crossroads.ca" };
                userManager.Create(user, "passW0rd");

                //Service, if we need one.

                context.Roles.AddOrUpdate(r => r.Name, new IdentityRole { Name = "Admin" });
                context.SaveChanges();

                userManager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
