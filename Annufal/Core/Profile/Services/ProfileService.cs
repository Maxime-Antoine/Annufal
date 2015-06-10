using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Annufal.Core.Profile
{
    public class ProfileService : IProfileService
    {
        public async Task<ProfileModel> GetByIdAsync(int id)
        {
            using (var db = new AppDbContext())
            {
                return await db.Profiles.FindAsync(id);
            }
        }

        public async Task<ProfileModel> GetForUserAsync(string userId)
        {
            using (var db = new AppDbContext())
            {
                return await db.Profiles.Where(p => p.UserId == userId).SingleOrDefaultAsync();
            }
        }

        public async Task<bool> CreateProfileAsync(ProfileModel profile)
        {
            using (var db = new AppDbContext())
            {
                db.Profiles.Add(profile);
                await db.SaveChangesAsync();

                return true;
            }
        }

        public async Task<bool> EditProfileAsync(ProfileModel profile)
        {
            using (var db = new AppDbContext())
            {
                db.Profiles.Attach(profile);
                db.Entry(profile).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return true;
            }
        }
    }
}