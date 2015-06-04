using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Annufal.Controllers.API
{
    [RoutePrefix("api/Test")]
    public class TestController : ApiController
    {
        [Route("Protected")]
        [Authorize()]
        public IHttpActionResult Protected()
        {
            return Ok();
        }

        [Route("NotProtected")]
        public IHttpActionResult NotProtected()
        {
            return Ok();
        }
    }
}
