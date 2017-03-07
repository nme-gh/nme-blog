namespace nme_blog.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<nme_blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(nme_blog.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin"});
            }

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Creating User for Nuran 
            if (!context.Users.Any(u => u.Email == "nuranme@gmail.com"))
            {
                userManager.Create(new ApplicationUser { UserName = "nuranme@gmail.com",
                    Email = "nuranme@gmail.com",
                    FirstName = "Nuran",
                    LastName = "Esen",
                    DisplayName = "NME"
                }, "metin716458");
            }

            var userId = userManager.FindByEmail("nuranme@gmail.com").Id;
            // Assigning User to a Role (User for Nuran, Role of Admin) 
            userManager.AddToRole(userId, "Admin");

            // Creating User for Jason 
            if (!context.Users.Any(u => u.Email == "jtwichell@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jtwichell@coderfoundry.com",
                    Email = "jtwichell@coderfoundry.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "JasonT"
                }, "password");
            }

            // Assigning User to a Role (User for Jason, Role of Moderator) 
            userId = userManager.FindByEmail("jtwichell@coderfoundry.com").Id;
            userManager.AddToRole(userId, "Moderator");

            // Creating User for Mark 
            if (!context.Users.Any(u => u.Email == "markjang@yahoo.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "markjang@yahoo.com",
                    Email = "markjang@yahoo.com",
                    FirstName = "Mark",
                    LastName = "Jaang",
                    DisplayName = "MarkJ"
                }, "password");
            }

            // Assigning User to a Role (User for Mark, Role of Moderator) 
            userId = userManager.FindByEmail("markjang@yahoo.com").Id;
            userManager.AddToRole(userId, "Moderator");

        }
    }
}
