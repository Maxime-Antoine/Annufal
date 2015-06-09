using Annufal.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Annufal.Controllers.API
{
    public abstract class BaseApiController : ApiController
    {
        private AuthModelFactory _modelFactory;
        private ApplicationUserManager _appUserManager = null;
        private ApplicationRoleManager _appRoleManager = null;

        protected ApplicationUserManager AppUserManager
        {
            get
            {
                return _appUserManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        protected ApplicationRoleManager AppRoleManager
        {
            get
            {
                return _appRoleManager ?? Request.GetOwinContext().Get<ApplicationRoleManager>();
            }
        }

        protected AuthModelFactory ModelFactory
        {
            get
            {
                if (_modelFactory == null)
                    _modelFactory = new AuthModelFactory(this.Request, this.AppUserManager);
                return _modelFactory;
            }
        }

        public BaseApiController()
        {
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
                return InternalServerError();

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                    foreach (string error in result.Errors)
                        ModelState.AddModelError("", error);

                if (ModelState.IsValid)
                    return BadRequest();

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
