using Annufal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annufal.Services
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
