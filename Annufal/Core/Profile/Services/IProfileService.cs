namespace Annufal.Core.Profile
{
    public interface IProfileService
    {
        void CreateProfile(ProfileModel profile);

        void EditProfile(ProfileModel profile);

        void ValidateProfile(string login);

        void RefuseProfile(string login);

        void DeleteProfile(string login);
    }
}