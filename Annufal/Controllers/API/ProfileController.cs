using Annufal.Core.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Annufal.Controllers.API
{
    //[Authorize]
    [RoutePrefix("api/profile")]
    public class ProfileController : BaseApiController
    {
        IProfileService _profileSvc;

        public ProfileController(IProfileService profileSvc)
        {
            _profileSvc = profileSvc;
        }

        [HttpGet]
        [Route("{profileId:int}")]
        public async Task<IHttpActionResult> Get(int profileId)
        {
            //TODO

            return null;
        }

        [HttpGet]
        [Route("{userId:guid}")]
        public async Task<IHttpActionResult> GetForUser(string userId)
        {
            //TODO

            return null;
        }

        [HttpPost]
        public async Task<IHttpActionResult> Create(CreateProfileBindingModel profile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //TODO

            return null;
        }

        [HttpPut]
        public async Task<IHttpActionResult> Edit()
        {
            //TODO

            return null;
        }
    }
}
