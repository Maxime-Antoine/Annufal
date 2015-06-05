using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Linq;
using System.Data.Entity;

namespace Annufal.Authentication
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext()
            : base("AuthContext")
        {
            //Database.SetInitializer(new AuthContextInitializer());
        }

        public static AuthDbContext Create()
        {
            return new AuthDbContext();
        }
    }

    //DEBUG
    internal class AuthContextInitializer : DropCreateDatabaseAlways<AuthDbContext>
    {
        protected override void Seed(AuthDbContext context)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new AuthDbContext()));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new AuthDbContext()));

            var user = new ApplicationUser()
            {
                UserName = "SuperUser",
                Email = "super.user@admin.com",
                EmailConfirmed = true,
                FirstName = "Super",
                LastName = "User",
                JoinDate = DateTime.Now.AddYears(-3)
            };

            manager.Create(user, "Super@dmin!");

            if (roleManager.Roles.Count() == 0)
            {
                roleManager.Create(new IdentityRole { Name = "SuperAdmin" });
                roleManager.Create(new IdentityRole { Name = "Admin" });
                roleManager.Create(new IdentityRole { Name = "User" });
            }

            var adminUser = manager.FindByName("SuperUser");

            manager.AddToRoles(adminUser.Id, new string[] { "SuperAdmin", "Admin" });

            base.Seed(context);
        }
    }
}