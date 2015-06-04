using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Annufal.Authentication
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext() : base("AuthContext")
        {
            //Database.SetInitializer(new AuthContextInitializer());
        }
    }

    //DEBUG
    public class AuthContextInitializer : DropCreateDatabaseAlways<AuthContext>
    {
        protected override void Seed(AuthContext context)
        {
            base.Seed(context);
        }
    }
}