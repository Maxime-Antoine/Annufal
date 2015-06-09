using Annufal.Core.Profile;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Annufal.Core
{
    public class AppDbContext : DbContext
    {
        DbSet<ProfileModel> Profiles { get; set; }
    }
}