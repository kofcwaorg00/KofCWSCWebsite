using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Azure;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;

namespace KofCWSCWebsite.Services
{
    public class Utils
    {
        public static string GetString(IHtmlContent content)
        {
            if (content == null)
                return null;

            using var writer = new System.IO.StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        public static string GetFratYearString()
        {
            // return the fraternal year in the form of xxxx - yyyy where xxxx is the 1st year of the fraternal year
            // and yyyy is the second year of the fraternal year
            // if the current year is odd then that is the yyyy
            string FratYearString = "";

            DateTime date = DateTime.Now;
            int currYear = date.Year;
            int prevYear = currYear - 1;
            int nextYear = currYear + 1;
            if (currYear % 2 == 0)
            {
                //year is odd
                FratYearString = prevYear.ToString() + "-" + currYear.ToString();
            }
            else
            {
                // year is even
                FratYearString = currYear.ToString() + "-" + nextYear.ToString();
            }
            return " " + FratYearString;
        }
     
        public static bool SendEmailAuthenticatedMG(string sTo, string sFrom,string sCC, string sBCC, string sSubject,string sBody, Attachment oAttachment)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(sFrom);
                mail.To.Add(sTo);
                if (sCC.Length > 0) { mail.CC.Add(sCC); }
                if (sBCC.Length > 0) { mail.Bcc.Add(sBCC); }
                if (oAttachment is not null) { mail.Attachments.Add(oAttachment); }
                mail.IsBodyHtml = true;
                mail.Subject = sSubject;
                mail.Body = sBody;

                SmtpClient client = new SmtpClient("smtp.mailgun.org");
                NetworkCredential cred = new NetworkCredential("postmaster@mg.kofc-wa.org", "0d794ba965a89f6775ba8d7e963dedde");
                client.Credentials = cred;
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Email Failed" + ex.Message + " - " + ex.InnerException);
                return false;
            }
        }
        public static bool SendEmailAuthenticatedAZ(string sTo, string sFrom, string sCC, string sBCC, string sSubject, string sBody, Attachment oAttachment)
        {


            string smtpAuthUsername = "KofCWSCCom|22875cd2-d6bf-4cb0-8ced-bbbb019a8572|cba846cf-1683-4d63-8c9c-93e37f653c83";
            string smtpAuthPassword = "NRU8Q~pkq5Zbt1OPvxNjGKIKPRQ_CMxDs6L~4cbq";
            string sender = sFrom;
            string recipient = sTo;
            string subject = sSubject;
            string body = sBody;

            string smtpHostUrl = "smtp.azurecomm.net";
            var client = new SmtpClient(smtpHostUrl)
            {
                Port = 587,
                Credentials = new NetworkCredential(smtpAuthUsername, smtpAuthPassword),
                EnableSsl = true
            };

            var message = new MailMessage(sender, recipient, subject, body);

            try
            {
                client.Send(message);
                Log.Information("Email Sent Success" + " - " + sTo + " - " + sFrom);
                return true;
                //Console.WriteLine("The email was successfully sent using Smtp.");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " - " + ex.InnerException);
                return false;
                //Console.WriteLine($"Smtp send failed with the exception: {ex.Message}.");
            }
        }
        public static string GetMemberNameLink(string DisplayName, int MemberID, bool isAuth,string ServerName,string ReturnURL)
        {
            try
            {
                if (MemberID < 0)
                {
                    throw new Exception("MemberID must be greater than 0");
                }
                
                if (isAuth)
                {
                    if (MemberID > 0)
                    {
                        string myHost = "https://" + ServerName + "/";
                        string myUri = string.Concat(myHost, "TblMasMembers/Details/", MemberID.ToString(), "?ReturnURL=", ReturnURL);
                        string myAhref = "<a href=" + myUri + ">" + DisplayName + "</a>";
                        return myAhref;
                    }
                    else
                    {
                        return DisplayName;
                    }
                }
                else
                {
                    return DisplayName;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " - " + ex.InnerException);
                return "";
            }
            
            
        }
    }
}
