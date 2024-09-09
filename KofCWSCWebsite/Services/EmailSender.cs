using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using Serilog;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Reflection;

namespace KofCWSCWebsite.Services
{
    public interface ISenderEmail
    {
        Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false);
    }

    public class EmailSender : ISenderEmail
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false)
        {
            try
            {
                string kvURL = _configuration["KV:VAULTURL"];
                var kvclient = new SecretClient(new Uri((string)kvURL), new DefaultAzureCredential());
                var vPassword = kvclient.GetSecret("EMAILPWD").Value;
                string Password = vPassword.Value;
                var vFromEmail = kvclient.GetSecret("EMAILUSER").Value;
                string FromEmail = vFromEmail.Value;

                string MailServer = _configuration["DASPEmailSettings:MailServer"];
                int Port = int.Parse(_configuration["DASPEmailSettings:MailPort"]);

                var client = new SmtpClient(MailServer, Port)
                {
                    Credentials = new NetworkCredential(FromEmail, Password),
                    EnableSsl = true,
                };

                MailMessage mailMessage = new MailMessage(FromEmail, ToEmail, Subject, Body)
                {
                    IsBodyHtml = IsBodyHtml
                };
                mailMessage.IsBodyHtml = true;
                return client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().Name + ex.Message + "-" + ex.InnerException);
                return Task.CompletedTask;
            }

        }
    }
}