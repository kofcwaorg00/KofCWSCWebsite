using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Azure;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Configuration;
using Azure;
using Azure.Communication.Email;
using sun.tools.tree;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using KofCWSCWebsite.Models;


namespace KofCWSCWebsite.Services
{
    public class Utils
    {
        public static string FormatLogEntry(object thisme,Exception ex,string addData = "")
        {
            //***********************************************************************************************
            // 12/05/2024 Tim Philomeno
            // trying to get a consistant logging string
            // usage: Log.Error(Utils.FormatLogEntry(this, ex));
            var method = new StackTrace(ex, true).GetFrame(0)?.GetMethod();
            var className = method?.DeclaringType?.FullName;
            DateTime date = DateTime.Now;
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return "WebApp" + " - " + env + " - " + date + " - " + thisme.GetType().Name + " - in method " + method + " - in class " + className + ex.Message + " - " + ex.InnerException + " - ***" + addData + "*** - "+ex.StackTrace;
            //-----------------------------------------------------------------------------------------------
        }
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

        // DASP should not be used anymore.  Switched over to use Azure Comm Server
        public static bool SendEmailAuthenticatedDASP(string sTo, string sFrom, string sCC, string sBCC, string sSubject, string sBody, 
            Attachment oAttachment,IConfiguration _config)
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

                string kvURL = _config["KV:VAULTURL"];
                var kvclient = new SecretClient(new Uri((string)kvURL), new DefaultAzureCredential());
                var vPassword = kvclient.GetSecret("DASPEMAILPWD").Value;
                string Password = vPassword.Value;
                var vFromEmail = kvclient.GetSecret("DASPEMAILUSER").Value;
                string FromEmail = vFromEmail.Value;

                string MailServer = _config["DASPEmailSettings:MailServer"];
                int Port = int.Parse(_config["DASPEmailSettings:MailPort"]);

                SmtpClient client = new SmtpClient(MailServer,Port);
                NetworkCredential cred = new NetworkCredential(FromEmail, Password);
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
        public static bool SendEmailAuthenticatedMG(string sTo, string sFrom,string sCC, string sBCC, string sSubject,string sBody, 
            Attachment oAttachment, IConfiguration _config)
        {
            try
            {
                sBody = sBody.Replace("\r\n", "<br />");
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(sFrom);
                mail.To.Add(sTo);
                if (sCC.Length > 0) { mail.CC.Add(sCC); }
                if (sBCC.Length > 0) { mail.Bcc.Add(sBCC); }
                if (oAttachment is not null) { mail.Attachments.Add(oAttachment); }
                mail.IsBodyHtml = true;
                mail.Subject = sSubject;
                mail.Body = sBody;

                string kvURL = _config["KV:VAULTURL"];
                var kvclient = new SecretClient(new Uri((string)kvURL), new DefaultAzureCredential());
                var vPassword = kvclient.GetSecret("MGEMAILPWD").Value;
                string Password = vPassword.Value;
                var vFromEmail = kvclient.GetSecret("MGEMAILUSER").Value;
                string FromEmail = vFromEmail.Value;

                string MailServer = _config["MGEmailSettings:MailServer"];
                int Port = int.Parse(_config["MGEmailSettings:MailPort"]);

                SmtpClient client = new SmtpClient(MailServer,Port);
                NetworkCredential cred = new NetworkCredential(FromEmail, Password);
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
        public static bool SendEmailAuthenticatedAZ(string sTo, string sFrom, string sCC, string sBCC, string sSubject, string sBody, 
            Attachment oAttachment, IConfiguration _config)
        {
            try
            {
                sBody = sBody.Replace("\r\n", "<br />");
                string kvURL = _config["KV:VAULTURL"];
                var kvclient = new SecretClient(new Uri((string)kvURL), new DefaultAzureCredential());
                var vConnString = kvclient.GetSecret("AZEmailConnString").Value;
                string connectionString = vConnString.Value;

                var emailClient = new EmailClient(connectionString);

                var emailMessage = new EmailMessage(
                    senderAddress: "DoNotReply@kofc-wa.org",
                    content: new EmailContent(sSubject)
                    {
                        Html = sBody
                    },
                    recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(sTo) }));

                EmailSendOperation emailSendOperation = emailClient.Send(
                    WaitUntil.Completed,
                    emailMessage);
                if (emailSendOperation != null)
                {
                    if (emailSendOperation.Value.Status == "Succeeded")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " - " + ex.InnerException);
                return false;
            }
            
            
            
            //string smtpAuthUsername = "KofCWSCEmailSvc.231618d3-e3bb-4d6d-8e11-920bfc3c90dc.cba846cf-1683-4d63-8c9c-93e37f653c83";
            //string smtpAuthPassword = "RUj8Q~Ha35tCQ80gzAjS72PYMQbmLLObW_QuVcdI";
            //string sender = sFrom;
            //string recipient = sTo;
            //string subject = sSubject;
            //string body = sBody;

            //string smtpHostUrl = "smtp.azurecomm.net";
            //var client = new SmtpClient(smtpHostUrl)
            //{
            //    Port = 587,
            //    Credentials = new NetworkCredential(smtpAuthUsername, smtpAuthPassword),
            //    EnableSsl = false,
               
            //};

            //var message = new MailMessage(sender, recipient, subject, body);

            //try
            //{
            //    client.Send(message);
            //    Log.Information("Email Sent Success" + " - " + sTo + " - " + sFrom);
            //    return true;
            //    //Console.WriteLine("The email was successfully sent using Smtp.");
            //}
            //catch (Exception ex)
            //{
            //    Log.Error(ex.Message + " - " + ex.InnerException);
            //    return false;
            //    //Console.WriteLine($"Smtp send failed with the exception: {ex.Message}.");
            //}
        }
        public static string FormatPhoneNumber(string? phoneNumber)
        {
            if (phoneNumber.IsNullOrEmpty())
            {
                return null;
                //return "(000) 000-0000";
            }
            else
            {
                // Remove any non-numeric characters
                string cleanedPhoneNumber = Regex.Replace(phoneNumber, @"\D", "");

                // Ensure the phone number has 10 digits
                if (cleanedPhoneNumber.Length == 10)
                {
                    // Format the phone number
                    string formattedPhoneNumber = $"({cleanedPhoneNumber.Substring(0, 3)}) {cleanedPhoneNumber.Substring(3, 3)}-{cleanedPhoneNumber.Substring(6, 4)}";
                    return formattedPhoneNumber;
                }
                else
                {
                    // Return the original input if it doesn't have 10 digits
                    return phoneNumber;
                }
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

        public static string CompareMemberInfo(CvnImpDelegate cvnImpDelegate)
        {
            string retval = "";
            // D1
            if (cvnImpDelegate.D1MemberID.ToString().IsNullOrEmpty()) { return "EMPTY"; }
            if (!(cvnImpDelegate.D1MemberID.ToString().IsNullOrEmpty() && cvnImpDelegate.ED1MemberID.ToString().IsNullOrEmpty() ) && cvnImpDelegate.D1MemberID != cvnImpDelegate.ED1MemberID) { retval += "D1IID;"; }
            if (!(cvnImpDelegate.D1FirstName.IsNullOrEmpty() && cvnImpDelegate.ED1FirstName.IsNullOrEmpty()) && cvnImpDelegate.D1FirstName != cvnImpDelegate.ED1FirstName) { retval += "D1IFN;"; }
            if (!(cvnImpDelegate.D1MiddleName.IsNullOrEmpty() && cvnImpDelegate.ED1MiddleName.IsNullOrEmpty()) && cvnImpDelegate.D1MiddleName != cvnImpDelegate.ED1MiddleName) { retval += "D1IMN;"; }
            if (!(cvnImpDelegate.D1LastName.IsNullOrEmpty() && cvnImpDelegate.ED1LastName.IsNullOrEmpty()) && cvnImpDelegate.D1LastName != cvnImpDelegate.ED1LastName) { retval += "D1ILN;"; }
            if (!(cvnImpDelegate.D1Suffix.IsNullOrEmpty() && cvnImpDelegate.ED1Suffix.IsNullOrEmpty()) && cvnImpDelegate.D1Suffix != cvnImpDelegate.ED1Suffix) { retval += "D1ISX;"; }
            if (!(cvnImpDelegate.D1Address1.IsNullOrEmpty() && cvnImpDelegate.ED1Address1.IsNullOrEmpty()) && cvnImpDelegate.D1Address1 != cvnImpDelegate.ED1Address1) { retval += "D1IAD;"; }
            if (!(cvnImpDelegate.D1City.IsNullOrEmpty() && cvnImpDelegate.ED1City.IsNullOrEmpty()) && cvnImpDelegate.D1City != cvnImpDelegate.ED1City) { retval += "D1ICT;"; }
            if (!(cvnImpDelegate.D1State.IsNullOrEmpty() && cvnImpDelegate.ED1State.IsNullOrEmpty()) && cvnImpDelegate.D1State != cvnImpDelegate.ED1State) { retval += "D1IST;"; }
            if (!(cvnImpDelegate.D1ZipCode.IsNullOrEmpty() && cvnImpDelegate.ED1ZipCode.IsNullOrEmpty()) && cvnImpDelegate.D1ZipCode != cvnImpDelegate.ED1ZipCode) { retval += "D1IZP;"; }
            if (!(cvnImpDelegate.D1Phone.IsNullOrEmpty() && cvnImpDelegate.ED1Phone.IsNullOrEmpty()) && cvnImpDelegate.D1Phone != cvnImpDelegate.ED1Phone) { retval += "D1IPH;"; }
            if (!(cvnImpDelegate.D1Email.IsNullOrEmpty() && cvnImpDelegate.ED1Email.IsNullOrEmpty()) && cvnImpDelegate.D1Email != cvnImpDelegate.ED1Email) { retval += "D1IEM;"; }
            return retval;
        }
    }
}
