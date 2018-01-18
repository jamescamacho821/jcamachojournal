namespace jcamacho_journal.Migrations
{
    using jcamacho_journal.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(jcamacho_journal.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //  context.People.AddOrUpdate(
            //     p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "jcamacho1964@optonline.net"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcamacho1964@optonline.net",
                    Email = "jcamacho1964@optonline.net",
                    FirstName = "James",
                    LastName = "Camacho",
                    DisplayName = "James Camacho"
                }, "empire111");
                var AdminId = userManager.FindByEmail("jcamacho1964@optonline.net").Id;
                userManager.AddToRole(AdminId, "Admin");

            }

            if (!context.Users.Any(u => u.Email == "moderator@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "moderator@coderfoundry.com",
                    Email = "moderator@coderfoundry.com",
                    FirstName = null,
                    LastName = null,
                    DisplayName = null
                }, "Password-1");
                var ModerId = userManager.FindByEmail("moderator@coderfoundry.com").Id;
                userManager.AddToRole(ModerId, "Moderator");
            }

            if (!context.Users.Any(u => u.Email == "ewatkins@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "ewatkins@coderfoundry.com",
                    Email = "ewatkins@coderfoundry.com",
                    FirstName = "Eric",
                    LastName = "Watkins",
                    DisplayName = "Eric Watkins"
                }, "password");
                var ModerId = userManager.FindByEmail("ewatkins@coderfoundry.com").Id;
                userManager.AddToRole(ModerId, "Moderator");
            }

        }
    }
}