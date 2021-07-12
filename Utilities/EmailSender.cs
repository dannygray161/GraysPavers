using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace GraysPavers.Utilities
{
    /*
     * here, we are using dependancy injection in order to retrieve our key and secret from app settings.json.
     *after we set up the private field and the ctor, we need a public field with a getter and setter
     * of type mailjetsettings. then we set _mailjetsettins = to _config
     * and we use .getsection("MailJet").get<mailjetsettings>() in order to pull the mailjet section from the json file
     * then inside of client, instead of passing the actual value of the key, we call _mailjetsettings.ApiKey, .ApiSecret
     * and tadaa everything works like it did when we had the literal values inside of the client.
     */





    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public MailJetSettings _mailJetSettings { get; set; }

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            _mailJetSettings = _configuration.GetSection("MailJet").Get<MailJetSettings>();
            var client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.ApiSecret);

            var request = new MailjetRequest { Resource = Send.Resource }
                .Property(Send.FromEmail, "danielgray161@protonmail.com")
                .Property(Send.FromName, "Daniel")
                .Property(Send.Subject, subject)
                .Property(Send.HtmlPart, body)
                .Property(Send.Recipients, new JArray
                {
                    new JObject
                    {
                        { "Email", email }
                    }
                });

            MailjetResponse response = await client.PostAsync(request);
        }
    }
}
