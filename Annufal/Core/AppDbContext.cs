using Annufal.Core.Profile;
using System.Data.Entity;

namespace Annufal.Core
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
            :base("AppContext")
        {
            Database.SetInitializer(new AppDbContextInitializer());
        }

        public DbSet<ProfileModel> Profiles { get; set; }

        public DbSet<ProfileValidationModel> ProfileValidations { get; set; }
    }

    public class AppDbContextInitializer : DropCreateDatabaseAlways<AppDbContext>
    {

    }
}