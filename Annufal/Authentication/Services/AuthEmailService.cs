using Microsoft.AspNet.Identity;
using Postal;
using System.Threading.Tasks;

namespace Annufal.Authentication
{
    public class AuthEmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            //await configSendGridasync(message);

            dynamic email = new Email("ConfirmRegistration");
            email.To = message.Destination;
            email.From = "hello@annufal.com";
            email.Subject = message.Subject;
            email.Body = message.Body;

            email.SendAsync();
        }
    }
}