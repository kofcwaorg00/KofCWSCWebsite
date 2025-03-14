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
        public static string FormatLogEntry(object thisme, Exception ex, string addData = "")
        {
            //***********************************************************************************************
            // 12/05/2024 Tim Philomeno
            // trying to get a consistant logging string
            // usage: Log.Error(Utils.FormatLogEntry(this, ex));
            var method = new StackTrace(ex, true).GetFrame(0)?.GetMethod();
            var className = method?.DeclaringType?.FullName;
            DateTime date = DateTime.Now;
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return "WebApp" + " - " + env + " - " + date + " - " + thisme.GetType().Name + " - in method " + method + " - in class " + className + ex.Message + " - " + ex.InnerException + " - ***" + addData + "*** - " + ex.StackTrace;
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
            Attachment oAttachment, IConfiguration _config)
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

                SmtpClient client = new SmtpClient(MailServer, Port);
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
        public static bool SendEmailAuthenticatedMG(string sTo, string sFrom, string sCC, string sBCC, string sSubject, string sBody,
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

                SmtpClient client = new SmtpClient(MailServer, Port);
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
        public static string GetMemberNameLink(string DisplayName, int MemberID, bool isAuth, string ServerName, string ReturnURL)
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
            // This method will look at specific properties and return a code that will
            // be used by the calling process to determine what, if any, properties are different
            string retval = "";
            //-----------------------------------------------------------------------------------------------------------------
            // D1
            //if (cvnImpDelegate.Validation.ToUpper().Contains("MISSING")) { return cvnImpDelegate.Validation; }
            if (cvnImpDelegate.ED1MemberID.ToString().IsNullOrEmpty())
            {
                retval = ";";
            }
            else
            {
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
            }
            //-----------------------------------------------------------------------------------------------------------------
            // D2
            //if (cvnImpDelegate.Validation.ToUpper().Contains("MISSING")) { return cvnImpDelegate.Validation; }
            if (cvnImpDelegate.ED2MemberID.ToString().IsNullOrEmpty())
            {
                retval = ";";
            }
            else
            {
                if (!(cvnImpDelegate.D2FirstName.IsNullOrEmpty() && cvnImpDelegate.ED2FirstName.IsNullOrEmpty()) && cvnImpDelegate.D2FirstName != cvnImpDelegate.ED2FirstName) { retval += "D2IFN;"; }
                if (!(cvnImpDelegate.D2MiddleName.IsNullOrEmpty() && cvnImpDelegate.ED2MiddleName.IsNullOrEmpty()) && cvnImpDelegate.D2MiddleName != cvnImpDelegate.ED2MiddleName) { retval += "D2IMN;"; }
                if (!(cvnImpDelegate.D2LastName.IsNullOrEmpty() && cvnImpDelegate.ED2LastName.IsNullOrEmpty()) && cvnImpDelegate.D2LastName != cvnImpDelegate.ED2LastName) { retval += "D2ILN;"; }
                if (!(cvnImpDelegate.D2Suffix.IsNullOrEmpty() && cvnImpDelegate.ED2Suffix.IsNullOrEmpty()) && cvnImpDelegate.D2Suffix != cvnImpDelegate.ED2Suffix) { retval += "D2ISX;"; }
                if (!(cvnImpDelegate.D2Address1.IsNullOrEmpty() && cvnImpDelegate.ED2Address1.IsNullOrEmpty()) && cvnImpDelegate.D2Address1 != cvnImpDelegate.ED2Address1) { retval += "D2IAD;"; }
                if (!(cvnImpDelegate.D2City.IsNullOrEmpty() && cvnImpDelegate.ED2City.IsNullOrEmpty()) && cvnImpDelegate.D2City != cvnImpDelegate.ED2City) { retval += "D2ICT;"; }
                if (!(cvnImpDelegate.D2State.IsNullOrEmpty() && cvnImpDelegate.ED2State.IsNullOrEmpty()) && cvnImpDelegate.D2State != cvnImpDelegate.ED2State) { retval += "D2IST;"; }
                if (!(cvnImpDelegate.D2ZipCode.IsNullOrEmpty() && cvnImpDelegate.ED2ZipCode.IsNullOrEmpty()) && cvnImpDelegate.D2ZipCode != cvnImpDelegate.ED2ZipCode) { retval += "D2IZP;"; }
                if (!(cvnImpDelegate.D2Phone.IsNullOrEmpty() && cvnImpDelegate.ED2Phone.IsNullOrEmpty()) && cvnImpDelegate.D2Phone != cvnImpDelegate.ED2Phone) { retval += "D2IPH;"; }
                if (!(cvnImpDelegate.D2Email.IsNullOrEmpty() && cvnImpDelegate.ED2Email.IsNullOrEmpty()) && cvnImpDelegate.D2Email != cvnImpDelegate.ED2Email) { retval += "D2IEM;"; }
            }
            //-----------------------------------------------------------------------------------------------------------------
            // A1
            //if (cvnImpDelegate.Validation.ToUpper().Contains("MISSING")) { return cvnImpDelegate.Validation; }
            if (cvnImpDelegate.EA1MemberID.ToString().IsNullOrEmpty())
            {
                retval = ";";
            }
            else
            {
                if (!(cvnImpDelegate.A1FirstName.IsNullOrEmpty() && cvnImpDelegate.EA1FirstName.IsNullOrEmpty()) && cvnImpDelegate.A1FirstName != cvnImpDelegate.EA1FirstName) { retval += "A1IFN;"; }
                if (!(cvnImpDelegate.A1MiddleName.IsNullOrEmpty() && cvnImpDelegate.EA1MiddleName.IsNullOrEmpty()) && cvnImpDelegate.A1MiddleName != cvnImpDelegate.EA1MiddleName) { retval += "A1IMN;"; }
                if (!(cvnImpDelegate.A1LastName.IsNullOrEmpty() && cvnImpDelegate.EA1LastName.IsNullOrEmpty()) && cvnImpDelegate.A1LastName != cvnImpDelegate.EA1LastName) { retval += "A1ILN;"; }
                if (!(cvnImpDelegate.A1Suffix.IsNullOrEmpty() && cvnImpDelegate.EA1Suffix.IsNullOrEmpty()) && cvnImpDelegate.A1Suffix != cvnImpDelegate.EA1Suffix) { retval += "A1ISX;"; }
                if (!(cvnImpDelegate.A1Address1.IsNullOrEmpty() && cvnImpDelegate.EA1Address1.IsNullOrEmpty()) && cvnImpDelegate.A1Address1 != cvnImpDelegate.EA1Address1) { retval += "A1IAD;"; }
                if (!(cvnImpDelegate.A1City.IsNullOrEmpty() && cvnImpDelegate.EA1City.IsNullOrEmpty()) && cvnImpDelegate.A1City != cvnImpDelegate.EA1City) { retval += "A1ICT;"; }
                if (!(cvnImpDelegate.A1State.IsNullOrEmpty() && cvnImpDelegate.EA1State.IsNullOrEmpty()) && cvnImpDelegate.A1State != cvnImpDelegate.EA1State) { retval += "A1IST;"; }
                if (!(cvnImpDelegate.A1ZipCode.IsNullOrEmpty() && cvnImpDelegate.EA1ZipCode.IsNullOrEmpty()) && cvnImpDelegate.A1ZipCode != cvnImpDelegate.EA1ZipCode) { retval += "A1IZP;"; }
                if (!(cvnImpDelegate.A1Phone.IsNullOrEmpty() && cvnImpDelegate.EA1Phone.IsNullOrEmpty()) && cvnImpDelegate.A1Phone != cvnImpDelegate.EA1Phone) { retval += "A1IPH;"; }
                if (!(cvnImpDelegate.A1Email.IsNullOrEmpty() && cvnImpDelegate.EA1Email.IsNullOrEmpty()) && cvnImpDelegate.A1Email != cvnImpDelegate.EA1Email) { retval += "A1IEM;"; }
            }
            //-----------------------------------------------------------------------------------------------------------------
            // A2
            //if (cvnImpDelegate.Validation.ToUpper().Contains("MISSING")) { return cvnImpDelegate.Validation; }
            if (cvnImpDelegate.EA2MemberID.ToString().IsNullOrEmpty())
            {
                retval = ";";
            }
            else
            {
                if (!(cvnImpDelegate.A2FirstName.IsNullOrEmpty() && cvnImpDelegate.EA2FirstName.IsNullOrEmpty()) && cvnImpDelegate.A2FirstName != cvnImpDelegate.EA2FirstName) { retval += "A2IFN;"; }
                if (!(cvnImpDelegate.A2MiddleName.IsNullOrEmpty() && cvnImpDelegate.EA2MiddleName.IsNullOrEmpty()) && cvnImpDelegate.A2MiddleName != cvnImpDelegate.EA2MiddleName) { retval += "A2IMN;"; }
                if (!(cvnImpDelegate.A2LastName.IsNullOrEmpty() && cvnImpDelegate.EA2LastName.IsNullOrEmpty()) && cvnImpDelegate.A2LastName != cvnImpDelegate.EA2LastName) { retval += "A2ILN;"; }
                if (!(cvnImpDelegate.A2Suffix.IsNullOrEmpty() && cvnImpDelegate.EA2Suffix.IsNullOrEmpty()) && cvnImpDelegate.A2Suffix != cvnImpDelegate.EA2Suffix) { retval += "A2ISX;"; }
                if (!(cvnImpDelegate.A2Address1.IsNullOrEmpty() && cvnImpDelegate.EA2Address1.IsNullOrEmpty()) && cvnImpDelegate.A2Address1 != cvnImpDelegate.EA2Address1) { retval += "A2IAD;"; }
                if (!(cvnImpDelegate.A2City.IsNullOrEmpty() && cvnImpDelegate.EA2City.IsNullOrEmpty()) && cvnImpDelegate.A2City != cvnImpDelegate.EA2City) { retval += "A2ICT;"; }
                if (!(cvnImpDelegate.A2State.IsNullOrEmpty() && cvnImpDelegate.EA2State.IsNullOrEmpty()) && cvnImpDelegate.A2State != cvnImpDelegate.EA2State) { retval += "A2IST;"; }
                if (!(cvnImpDelegate.A2ZipCode.IsNullOrEmpty() && cvnImpDelegate.EA2ZipCode.IsNullOrEmpty()) && cvnImpDelegate.A2ZipCode != cvnImpDelegate.EA2ZipCode) { retval += "A2IZP;"; }
                if (!(cvnImpDelegate.A2Phone.IsNullOrEmpty() && cvnImpDelegate.EA2Phone.IsNullOrEmpty()) && cvnImpDelegate.A2Phone != cvnImpDelegate.EA2Phone) { retval += "A2IPH;"; }
                if (!(cvnImpDelegate.A2Email.IsNullOrEmpty() && cvnImpDelegate.EA2Email.IsNullOrEmpty()) && cvnImpDelegate.A2Email != cvnImpDelegate.EA2Email) { retval += "A2IEM;"; }
            }
            return retval;
        }
    }
}
