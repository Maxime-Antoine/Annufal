using Annufal.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Annufal.Controllers.API
{
    //[Authorize(Roles = "Admin")]
    [RoutePrefix("api/roles")]
    public class RoleController : BaseApiController
    {
        [Route("{id:guid}", Name = "GetRoleById")]
        public async Task<IHttpActionResult> GetRoleById(string id)
        {
            var role = await this.AppRoleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            return Ok(this.ModelFactory.Create(role));
        }

        [Route("", Name = "GetAllRoles")]
        public IHttpActionResult GetAllRoles()
        {
            var roles = this.AppRoleManager.Roles;

            return Ok(roles);
        }

        [Route("create")]
        public async Task<IHttpActionResult> Create(CreateRoleBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = new IdentityRole { Name = model.Name };
            
            var result = await this.AppRoleManager.CreateAsync(role);

            if (!result.Succeeded)
                return GetErrorResult(result);

            Uri locationHeader = new Uri(Url.Link("GetRoleById", new { id = role.Id }));

            return Created(locationHeader, this.ModelFactory.Create(role));
        }

        [Route("{id:guid}")]
        public async Task<IHttpActionResult> DeleteRole(string id)
        {
            var role = await this.AppRoleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            IdentityResult result = await this.AppRoleManager.DeleteAsync(role);

            if (!result.Succeeded)
                return GetErrorResult(result);

            return Ok();
        }

        [Route("ManageUsersInRole")]
        public async Task<IHttpActionResult> ManageUsersInRole(UsersInRoleModel model)
        {
            var role = await this.AppRoleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist");
                return BadRequest(ModelState);
            }

            foreach (string user in model.EnrolledUsers)
            {
                var appUser = await this.AppUserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", string.Format("User: {0} does not exist.", user));
                    continue;
                }

                if (!this.AppUserManager.IsInRole(user, role.Name))
                {
                    IdentityResult result = await this.AppUserManager.AddToRoleAsync(user, role.Name);

                    if (!result.Succeeded)
                        ModelState.AddModelError("", String.Format("User: {0} could not be added to role {1}.", user, role.Name));  
                }
            }

            foreach (string user in model.RemovedUsers)
            {
                var appUser = await this.AppUserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", string.Format("User: {0} does not exist.", user));
                    continue;
                }

                IdentityResult result = await this.AppUserManager.RemoveFromRoleAsync(user, role.Name);

                if (!result.Succeeded)
                    ModelState.AddModelError("", string.Format("User: {0} could not be removed from role {1}.", user, role.Name));
            }

            if (ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok();
        }
    }
}
