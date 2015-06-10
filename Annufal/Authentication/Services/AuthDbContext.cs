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
            Database.SetInitializer(new AuthContextInitializer());
        }

        public static AuthDbContext Create()
        {
            return new AuthDbContext();
        }
    }

    internal class AuthContextInitializer : DropCreateDatabaseIfModelChanges<AuthDbContext>
    {
        protected override void Seed(AuthDbContext context)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var user = new ApplicationUser()
            {
                UserName = "SuperUser",
                Email = "super.user@admin.com",
                EmailConfirmed = true,
                JoinDate = DateTime.Now.AddYears(-3)
            };

            manager.Create(user, "Super@dmin42");

            if (roleManager.Roles.Count() == 0)
            {
                roleManager.Create(new IdentityRole { Name = "SuperAdmin" });
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            var adminUser = manager.FindByName("SuperUser");

            manager.AddToRoles(adminUser.Id, new string[] { "SuperAdmin", "Admin" });

            base.Seed(context);
        }
    }
}