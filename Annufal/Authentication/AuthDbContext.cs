using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Annufal.Authentication
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext() : base("AuthContext")
        {
            //Database.SetInitializer(new AuthContextInitializer());
        }

        public static AuthDbContext Create()
        {
            return new AuthDbContext();
        }
    }

    //DEBUG
    public class AuthContextInitializer : DropCreateDatabaseAlways<AuthDbContext>
    {
        protected override void Seed(AuthDbContext context)
        {
            base.Seed(context);
        }
    }
}