using Annufal.Core.Profile;
using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Annufal.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/profile")]
    public class ProfileController : BaseApiController
    {
        private IProfileService _profileSvc;

        public ProfileController(IProfileService profileSvc)
        {
            _profileSvc = profileSvc;
        }

        [HttpGet]
        [Route("{profileId:int}")]
        public async Task<IHttpActionResult> Get(int profileId)
        {
            var profile = await _profileSvc.GetByIdAsync(profileId);

            if (profile == null)
                return NotFound();
            else
                return Ok(profile);
        }

        [HttpGet]
        [Route("{userId:guid}")]
        public async Task<IHttpActionResult> GetForUser(string userId)
        {
            var profile = await _profileSvc.GetForUserAsync(userId);

            if (profile == null)
                return NotFound();
            else
                return Ok(profile);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Create(CreateProfileBindingModel profileBindingModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //check that this user has no profile so far
            var curProfile = await _profileSvc.GetForUserAsync(User.Identity.GetUserId());
            if (curProfile != null)
                return BadRequest("This user has already a profile");

            //else create the profile
            var profile = Mapper.Map<CreateProfileBindingModel, ProfileModel>(profileBindingModel);

            //complete data
            profile.Status = EProfileStatus.New;
            profile.CreationTimeStamp = DateTime.Now;
            profile.UserId = User.Identity.GetUserId();

            await _profileSvc.CreateProfileAsync(profile);

            return Ok();
        }

        [HttpPut]
        public async Task<IHttpActionResult> Edit()
        {
            //TODO

            return null;
        }
    }
}