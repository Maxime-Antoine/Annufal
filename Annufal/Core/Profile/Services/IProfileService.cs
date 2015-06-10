using System.Threading.Tasks;
namespace Annufal.Core.Profile
{
    public interface IProfileService
    {
        Task<ProfileModel> GetByIdAsync(int id);

        Task<ProfileModel> GetForUserAsync(string userId);

        Task<bool> CreateProfileAsync(ProfileModel profile);

        Task<bool> EditProfileAsync(ProfileModel profile);
    }
}