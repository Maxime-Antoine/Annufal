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
        public Task<IHttpActionResult> Get(int profileId)
        {
            //TODO

            return null;
        }

        [HttpGet]
        [Route("{userId:guid}")]
        public Task<IHttpActionResult> GetForUser(string userId)
        {
            //TODO

            return null;
        }

        [HttpPost]
        public Task<IHttpActionResult> Create()
        {
            //TODO

            return null;
        }

        [HttpPut]
        public Task<IHttpActionResult> Edit()
        {
            //TODO

            return null;
        }
    }
}
