using Microsoft.AspNet.Identity;
using Postal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Annufal.Authentication
{
    public class EmailService : IIdentityMessageService
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

        //private async Task configSendGridasync(IdentityMessage message)
        //{
        //    var myMessage = new SendGridMessage();

        //    myMessage.AddTo(message.Destination);
        //    myMessage.From = new System.Net.Mail.MailAddress("hello@annufal.com", "AnnuFal");
        //    myMessage.Subject = message.Subject;
        //    myMessage.Text = message.Body;
        //    myMessage.Html = message.Body;

        //    var credentials = new NetworkCredential(ConfigurationManager.AppSettings["emailService:Account"],
        //                                            ConfigurationManager.AppSettings["emailService:Password"]);

        //    var transportWeb = new Web(credentials);

        //    if (transportWeb != null)
        //        await transportWeb.DeliverAsync(myMessage);
        //    else
        //        await Task.FromResult(0);
        //}
    }
}