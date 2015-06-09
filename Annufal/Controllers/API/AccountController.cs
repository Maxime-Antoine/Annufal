using Annufal.Authentication;
using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Annufal.Controllers.API
{
    [Authorize]
    [RoutePrefix("api/account")]
    public class AccountController : BaseApiController
    {
        [Authorize(Roles = "Admin")]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.ModelFactory.Create(u)));
        }

        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUserById(string id)
        {
            var user = await this.AppUserManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();
            else
                return Ok(this.ModelFactory.Create(user));
        }

        [Authorize(Roles = "Admin")]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user == null)
                return NotFound();
            else
                return Ok(this.ModelFactory.Create(user));
        }

        [AllowAnonymous]
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                JoinDate = DateTime.Now.Date
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
                return GetErrorResult(addUserResult);

            //send confirmation email
            string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));
            await this.AppUserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking " + callbackUrl);

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, ModelFactory.Create(user));
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
                return Ok();
            else
                return GetErrorResult(result);
        }

        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
                return GetErrorResult(result);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser == null)
                return NotFound();

            IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

            if (!result.Succeeded)
                return GetErrorResult(result);

            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}/roles")]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign)
        {
            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser == null)
                return NotFound();

            var currentRoles = await this.AppUserManager.GetRolesAsync(appUser.Id);

            var rolesNotExists = rolesToAssign.Except(this.AppRoleManager.Roles.Select(x => x.Name)).ToArray();

            if (rolesNotExists.Count() > 0)
            {
                ModelState.AddModelError("", string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
                return BadRequest(ModelState);
            }

            IdentityResult removeResult = await this.AppUserManager.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray());

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return BadRequest(ModelState);
            }

            IdentityResult addResult = await this.AppUserManager.AddToRolesAsync(appUser.Id, rolesToAssign);

            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add user roles");
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("resetPassword/{username}")]
        public async Task<IHttpActionResult> ResetPassword(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user == null)
                return NotFound();

            var token = await this.AppUserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = new Uri(Url.Link("ConfirmResetPassword", new { userId = user.Id, token = token }));
            await this.AppUserManager.SendEmailAsync(user.Id, "Reset password", "Please confirm you want to reset your password by clicking " + callbackUrl);

            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("confirmResetPassword", Name = "ConfirmResetPassword")]
        public async Task<IHttpActionResult> ConfirmResetPassword(string userId, string token)
        {
            var newPwd = "P@ssw0rd";

            var resetPwdResult = await this.AppUserManager.ResetPasswordAsync(userId, token, newPwd);

            if (!resetPwdResult.Succeeded)
                return GetErrorResult(resetPwdResult);

            await this.AppUserManager.SendEmailAsync(userId, "Password reset", "Your password has been reset to : " + newPwd + ". Please change it ASAP.");

            var redirectUrl = ConfigurationManager.AppSettings["AppRootUrl"].ToString() + "/#/passwordReset";

            var response = new HttpResponseMessage(HttpStatusCode.Redirect);
            response.Headers.Location = new Uri(redirectUrl);

            throw new HttpResponseException(response);
        }
    }
}