namespace Annufal.Core.Profile
{
    public interface IProfileService
    {
        void CreateProfile(CreateProfileBindingModel profile);

        void EditProfile(CreateProfileBindingModel profile);

        void ValidateProfile(string login);

        void RefuseProfile(string login);

        void DeleteProfile(string login);
    }
}