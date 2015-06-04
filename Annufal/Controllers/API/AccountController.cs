﻿using Annufal.Authentication;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Annufal.API.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private AuthRepository _repo = null;

        public AccountController()
        {
            _repo = new AuthRepository();
        }

        //POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserModel userModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityResult result = await _repo.RegisterUser(userModel);

            IHttpActionResult errorResult = _GetErrorResult(result);

            if (errorResult != null)
                return errorResult;

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _repo.Dispose();

            base.Dispose(disposing);
        }

        private IHttpActionResult _GetErrorResult(IdentityResult result)
        {
            if (result == null)
                return InternalServerError();

            if (!result.Succeeded)
            {
                if (result.Errors.Count() != 0)
                    foreach (string error in result.Errors)
                        ModelState.AddModelError("", error);

                if (ModelState.IsValid) // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}