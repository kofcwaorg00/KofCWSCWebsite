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
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using KofCWSCWebsite.Areas.Identity.Data;
using System.Reflection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using com.sun.imageio.plugins.common;


namespace KofCWSCWebsite.Services
{
    public class Utils
    {
        public static string GetPicImage(string url,int w,int h,string mname,int KofCID,ClaimsPrincipal user)
        {
            // url = missingA and myPicURL = defaultprofilepics "profile missing but we hvae a default in wwwroot"
            // url = null and myPicURL=missing.png
            // url = missingA and myPIcURL = missingA
            // url = null and myPicURL = defaultprofilepics "has profile but is missing pictureurl and we hve default in wwwroot"
            // url = blob and myPicURL = blob "has profile picture
            string myUrl = url.IsNullOrEmpty() ? string.Empty : url;
            string mytitle = string.Empty;
            string myPicURL = ProcessPicURL(url,KofCID);
            string myBorder = string.Empty;
            
            if (myUrl.IsNullOrEmpty() && myPicURL.Contains("defaultprofilepics"))
            {
                mytitle = $"{mname} - Member has a profile but no Picture is present. Using default picture in wwwroot";
            }
            if (myUrl.Contains("missingA") && myPicURL.Contains("defaultprofilepics"))
            {
                myBorder = user.IsInRole("Admin") ? "border:solid;border-color:red" : string.Empty;
                mytitle = user.IsInRole("Admin") ? $"{mname} - Profile is missing but we have a default picture in wwwroot" : mname;
            }
            if (myUrl.IsNullOrEmpty() && myPicURL.Contains("missing.png"))
            {
                mytitle = user.IsInRole("Admin") ? $"{mname} - Member has a profile but no Picture is present. No picture found in wwwroot" : mname;
            }
            if (myUrl.Contains("missingA") && myPicURL.Contains("missingA"))
            {
                // add the red to report to ADMIN that the profile is missing
                myBorder = user.IsInRole("Admin") ? "border:solid;border-color:red" : string.Empty;
                mytitle = user.IsInRole("Admin") ? $"{mname} - Profile is missing. No picture found in wwwroot" : mname;
            }
            if (myUrl.Contains("blob"))
            {
                mytitle = mname;
            }
                       
            string myH = h == 0 ? string.Empty : $"height:{h}px;";
            //string myImage = $"<img src='{myPicURL}' alt='Profile Picture' class='card-img-top' " +
            //    $"title='{mytitle}' style='width:{w}px; {myH}' object-fit: 'cover';background-color:'white' {myBorder} />";
            //string myImage = $"<img src='{myPicURL}' alt='Profile Picture' class='card-img-top' " +
            //    $"title='{mytitle}' style='width:{w}px; {myH}' object-fit: 'cover';background-color:'white' {myBorder} />";

            var imgTag = new TagBuilder("img");
            imgTag.Attributes.Add("src", myPicURL);
            imgTag.Attributes.Add("alt", "Profile Picture");
            imgTag.Attributes.Add("style", $"width:{w}px; {myH} object-fit: cover; background-color: white;{myBorder}");
            imgTag.Attributes.Add("title", mytitle);

            using var writer = new StringWriter();
            imgTag.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            return writer.ToString();

        }
        public static string ProcessPicURL(string url,int KofCID)
        {
            // url will contain
            //  NULL - found profile but pic url is not there send back missing.png
            //  default - no profile found so defaultprofilepic is returned
            //  not null send back the url

            // if it is null return a default missing png or default nnnnnn.png
            if (url == null) {

                string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "defaultprofilepics");
                string imagePath = Path.Combine(webRootPath, Path.GetFileName($"/{KofCID}.png"));

                if (File.Exists(imagePath))
                {
                    return $"/images/defaultprofilepics/{KofCID}.png";
                }
                else
                {
                    return "/images/missing.png";
                }
            }
            // if it is the default, test for it's exsistance and return the path
            if (url.Contains("missingA"))
            {
                // parse the imagename from the url
                // check to see if it exists
                string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images","defaultprofilepics");
                string imagePath = Path.Combine(webRootPath, Path.GetFileName($"/{KofCID}.png"));

                if (File.Exists(imagePath))
                {
                    return $"/images/defaultprofilepics/{KofCID}.png";
                }
                else
                {
                    return "/images/missingA.png";
                }
            }
            return url;
        }
        public async static Task<T?> GetUserProp<T>(ClaimsPrincipal cpuser, Microsoft.AspNetCore.Identity.UserManager<KofCUser> _userManager,string property)
        {
            // Step 1: Retrieve the user ID from the ClaimsPrincipal
            var userId = cpuser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return default; // Return null if no user ID is found
            }

            // Step 2: Use UserManager to fetch the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return default; // Return null if user is not found
            }

            // Step 3: Use reflection to get the property value
            var propertyInfo = typeof(KofCUser).GetProperty(property);
            if (propertyInfo == null)
            {
                return default; // Return null if the property does not exist
            }

            var value = propertyInfo.GetValue(user);

            // Step 4: Ensure the value can be cast to the desired type
            if (value is T typedValue)
            {
                return typedValue;
            }

            return default; // Return null if casting fails
        }

        //public async static Task<T?> GetUserProp<T>(ClaimsPrincipal cpuser, Microsoft.AspNetCore.Identity.UserManager<KofCUser> _userManager,string property)
        //{
        //    var userId = cpuser.Identity.Name;
        //    var user = await _userManager.FindByIdAsync(userId);
        //    return user.KofCMemberID;
        //}
        public static Attachment ConvertToNetMailAttachment(IFormFile formFile)
        {
            if (formFile == null)
            {
                throw new ArgumentNullException(nameof(formFile), "FormFile cannot be null.");
            }

            // Create a memory stream to hold the file's content
            MemoryStream memoryStream = new MemoryStream();
            formFile.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Create and return the System.Net.Mail.Attachment
            return new Attachment(memoryStream, formFile.FileName, formFile.ContentType);
        }




        private static EmailAttachment ConvertToAZEmailAttachment(IFormFile attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment));

            // Create a MemoryStream to hold the attachment content
            using var memoryStream = new MemoryStream();
            attachment.OpenReadStream().CopyTo(memoryStream);
            memoryStream.Position = 0;  // Reset the position of the stream after copying

            // Create the MIME type for the attachment
            string mimeType = attachment.ContentType;

            // Convert the content stream to BinaryData
            var binaryData = BinaryData.FromStream(memoryStream);

            // Create and return the Azure EmailAttachment
            var emailAttachment = new EmailAttachment(
                attachment.FileName,   // The name of the file
                mimeType,              // The MIME type (string)
                binaryData             // The BinaryData object (attachment content)
            );

            return emailAttachment;
        }

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
            IFormFile oAttachment, IConfiguration _config)
        {
            try
            {
                MailMessage mail = new MailMessage();

                if (oAttachment != null)
                {
                    mail.Attachments.Add(Utils.ConvertToNetMailAttachment(oAttachment));
                }



                sBody = sBody.Replace("\r\n", "<br />");
                //MailMessage mail = new MailMessage();
                mail.From = new MailAddress(sFrom);
                mail.To.Add(sTo);
                if (sCC.Length > 0) { mail.CC.Add(sCC); }
                if (sBCC.Length > 0) { mail.Bcc.Add(sBCC); }
                //if (oAttachment is not null) { mail.Attachments.Add(myAttachment); }
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
            IFormFile oAttachment, IConfiguration _config)
        {
            try
            {
                var emailMessage = new EmailMessage(
                    senderAddress: "DoNotReply@kofc-wa.org",
                    content: new EmailContent(sSubject)
                    {
                        Html = sBody
                    },
                    recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(sTo) }));

                if (oAttachment != null)
                {
                    var emailAttachment = ConvertToAZEmailAttachment(oAttachment);
                    emailMessage.Attachments.Add(emailAttachment);
                }




                sBody = sBody.Replace("\r\n", "<br />");
                string kvURL = _config["KV:VAULTURL"];
                var kvclient = new SecretClient(new Uri((string)kvURL), new DefaultAzureCredential());
                var vConnString = kvclient.GetSecret("AZEmailConnString").Value;
                string connectionString = vConnString.Value;

                var emailClient = new EmailClient(connectionString);

                //var emailMessage = new EmailMessage(
                //    senderAddress: "DoNotReply@kofc-wa.org",
                //    content: new EmailContent(sSubject)
                //    {
                //        Html = sBody
                //    },
                //    recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(sTo) }));

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
            if (!cvnImpDelegate.ED1MemberID.ToString().IsNullOrEmpty())
            {
                if (cvnImpDelegate.CouncilNumber != cvnImpDelegate.ED1Council) { retval += "D1ICN;"; }
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
            if (!cvnImpDelegate.ED2MemberID.ToString().IsNullOrEmpty())
            {
                if (cvnImpDelegate.CouncilNumber != cvnImpDelegate.ED2Council) { retval += "D2ICN;"; }
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
            if (!cvnImpDelegate.EA1MemberID.ToString().IsNullOrEmpty())
            {
                if (cvnImpDelegate.CouncilNumber != cvnImpDelegate.EA1Council) { retval += "A1ICN;"; }
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
            if (!cvnImpDelegate.EA2MemberID.ToString().IsNullOrEmpty())
            {
                if (cvnImpDelegate.CouncilNumber != cvnImpDelegate.EA2Council) { retval += "A2ICN;"; }
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
            return retval.Length == 0 ? "NONE" : retval;
        }

        public static string GetStateAbbr(string stateName)
        {
            if (stateName.Length == 2) { return stateName; }
            var states = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Alabama", "AL" },
                { "Alaska", "AK" },
                { "Arizona", "AZ" },
                { "Arkansas", "AR" },
                { "California", "CA" },
                { "Colorado", "CO" },
                { "Connecticut", "CT" },
                { "Delaware", "DE" },
                { "Florida", "FL" },
                { "Georgia", "GA" },
                { "Hawaii", "HI" },
                { "Idaho", "ID" },
                { "Illinois", "IL" },
                { "Indiana", "IN" },
                { "Iowa", "IA" },
                { "Kansas", "KS" },
                { "Kentucky", "KY" },
                { "Louisiana", "LA" },
                { "Maine", "ME" },
                { "Maryland", "MD" },
                { "Massachusetts", "MA" },
                { "Michigan", "MI" },
                { "Minnesota", "MN" },
                { "Mississippi", "MS" },
                { "Missouri", "MO" },
                { "Montana", "MT" },
                { "Nebraska", "NE" },
                { "Nevada", "NV" },
                { "New Hampshire", "NH" },
                { "New Jersey", "NJ" },
                { "New Mexico", "NM" },
                { "New York", "NY" },
                { "North Carolina", "NC" },
                { "North Dakota", "ND" },
                { "Ohio", "OH" },
                { "Oklahoma", "OK" },
                { "Oregon", "OR" },
                { "Pennsylvania", "PA" },
                { "Rhode Island", "RI" },
                { "South Carolina", "SC" },
                { "South Dakota", "SD" },
                { "Tennessee", "TN" },
                { "Texas", "TX" },
                { "Utah", "UT" },
                { "Vermont", "VT" },
                { "Virginia", "VA" },
                { "Washington", "WA" },
                { "West Virginia", "WV" },
                { "Wisconsin", "WI" },
                { "Wyoming", "WY" }
            };

            if (states.TryGetValue(stateName, out string abbreviation))
            {
                return abbreviation;
            }
            else
            {
                throw new ArgumentException($"State name '{stateName}' is not recognized.");
            }
        }

        private static bool ShouldUpdate(string? inItem, string? exItem, TblMasMember member, string what, bool isNew)
        {
            // if it is a new member do the update
            if (isNew) { return true; }
            // if both items are empty or null do nothing
            if (inItem.IsNullOrEmpty() && exItem.IsNullOrEmpty()) { return false; }
            // if one is empty but the other is not do the update
            if (inItem.IsNullOrEmpty() && !exItem.IsNullOrEmpty()) { return true; }
            // if one is empty but the other is not do the update
            if (!inItem.IsNullOrEmpty() && exItem.IsNullOrEmpty()) { return true; }
            // if we get here both have a value and we need to check and do the right thing
            if (exItem.ToUpper() != inItem.ToUpper())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool FillD1ImpDel(ref CvnImpDelegateIMP myDelegateImp, CvnImpDelegate myDelegate)
        {
            myDelegateImp.D1MemberID = (int)myDelegate.D1MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            myDelegateImp.D1FirstName = myDelegate.D1FirstName;
            myDelegateImp.D1LastName = myDelegate.D1LastName;
            myDelegateImp.D1MiddleName = myDelegate.D1MiddleName;
            myDelegateImp.D1Suffix = myDelegate.D1Suffix;
            myDelegateImp.D1Address1 = myDelegate.D1Address1;
            myDelegateImp.D1City = myDelegate.D1City;
            myDelegateImp.D1State = myDelegate.D1State;
            myDelegateImp.D1ZipCode = myDelegate.D1ZipCode;
            myDelegateImp.D1Phone = myDelegate.D1Phone;
            myDelegateImp.D1Email = myDelegate.D1Email;
            return true;
        }
        public static bool FillD2ImpDel(ref CvnImpDelegateIMP myDelegateImp, CvnImpDelegate myDelegate)
        {
            myDelegateImp.D2MemberID = (int)myDelegate.D2MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            myDelegateImp.D2FirstName = myDelegate.D2FirstName;
            myDelegateImp.D2LastName = myDelegate.D2LastName;
            myDelegateImp.D2MiddleName = myDelegate.D2MiddleName;
            myDelegateImp.D2Suffix = myDelegate.D2Suffix;
            myDelegateImp.D2Address1 = myDelegate.D2Address1;
            myDelegateImp.D2City = myDelegate.D2City;
            myDelegateImp.D2State = myDelegate.D2State;
            myDelegateImp.D2ZipCode = myDelegate.D2ZipCode;
            myDelegateImp.D2Phone = myDelegate.D2Phone;
            myDelegateImp.D2Email = myDelegate.D2Email;
            return true;
        }
        public static bool FillA1ImpDel(ref CvnImpDelegateIMP myDelegateImp, CvnImpDelegate myDelegate)
        {
            myDelegateImp.A1MemberID = (int)myDelegate.A1MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            myDelegateImp.A1FirstName = myDelegate.A1FirstName;
            myDelegateImp.A1LastName = myDelegate.A1LastName;
            myDelegateImp.A1MiddleName = myDelegate.A1MiddleName;
            myDelegateImp.A1Suffix = myDelegate.A1Suffix;
            myDelegateImp.A1Address1 = myDelegate.A1Address1;
            myDelegateImp.A1City = myDelegate.A1City;
            myDelegateImp.A1State = myDelegate.A1State;
            myDelegateImp.A1ZipCode = myDelegate.A1ZipCode;
            myDelegateImp.A1Phone = myDelegate.A1Phone;
            myDelegateImp.A1Email = myDelegate.A1Email;
            return true;
        }
        public static bool FillA2ImpDel(ref CvnImpDelegateIMP myDelegateImp, CvnImpDelegate myDelegate)
        {
            myDelegateImp.A2MemberID = (int)myDelegate.A2MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            myDelegateImp.A2FirstName = myDelegate.A2FirstName;
            myDelegateImp.A2LastName = myDelegate.A2LastName;
            myDelegateImp.A2MiddleName = myDelegate.A2MiddleName;
            myDelegateImp.A2Suffix = myDelegate.A2Suffix;
            myDelegateImp.A2Address1 = myDelegate.A2Address1;
            myDelegateImp.A2City = myDelegate.A2City;
            myDelegateImp.A2State = myDelegate.A2State;
            myDelegateImp.A2ZipCode = myDelegate.A2ZipCode;
            myDelegateImp.A2Phone = myDelegate.A2Phone;
            myDelegateImp.A2Email = myDelegate.A2Email;
            return true;
        }
        public static bool FillD1(ref TblMasMember myMember, CvnImpDelegate myDelegate)
        {
            string UpdatedBy = "Delegate Import API";
            bool isUpdated = false;
            bool isNew = false;

            if (myMember == null)
            {
                myMember = new TblMasMember();
                isNew = true;
            }

            // if myMember is null that means that we are adding a new one so we need to
            // spin up a new model object and skip memberid assignmane
            // if we have an exitsting incomning member recored it's MemberID will already
            // be set so don't mess with it
            // myMember.MemberId = ??

            myMember.KofCid = (int)myDelegate.D1MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            // COUNCIL
            if (myMember.Council != (int)myDelegate.CouncilNumber)
            {
                myMember.Council = (int)myDelegate.CouncilNumber;
                myMember.CouncilUpdated = DateTime.Now;
                myMember.CouncilUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // FIRSTNAME
            if (ShouldUpdate(myDelegate.D1FirstName, myMember.FirstName, myMember, "FirstName", isNew))
            {
                myMember.FirstName = myDelegate.D1FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (ShouldUpdate(myDelegate.D1LastName, myMember.LastName, myMember, "LastName", isNew))
            {
                myMember.LastName = myDelegate.D1LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (ShouldUpdate(myDelegate.D1MiddleName, myMember.MI, myMember, "MiddleName", isNew))
            {
                myMember.MI = myDelegate.D1MiddleName;
                myMember.MIUpdated = DateTime.Now;
                myMember.MIUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (ShouldUpdate(myDelegate.D1Suffix, myMember.Suffix, myMember, "Suffix", isNew))
            {
                myMember.Suffix = myDelegate.D1Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (ShouldUpdate(myDelegate.D1Address1, myMember.Address, myMember, "Address", isNew))
            {
                myMember.Address = myDelegate.D1Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddressUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (ShouldUpdate(myDelegate.D1City, myMember.City, myMember, "City", isNew))
            {
                myMember.City = myDelegate.D1City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (ShouldUpdate(GetStateAbbr(myDelegate.D1State), myMember.State, myMember, "State", isNew))
            {
                myMember.State = GetStateAbbr(myDelegate.D1State);
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (ShouldUpdate(myDelegate.D1ZipCode, myMember.PostalCode, myMember, "PostalCode", isNew))
            {
                myMember.PostalCode = myDelegate.D1ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (ShouldUpdate(myDelegate.D1Phone, myMember.Phone, myMember, "Phone", isNew))
            {
                myMember.Phone = myDelegate.D1Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (ShouldUpdate(myDelegate.D1Email, myMember.Email, myMember, "Email", isNew))
            {
                myMember.Email = myDelegate.D1Email;
                myMember.EmailUpdated = DateTime.Now;
                myMember.EmailUpdatedBy = UpdatedBy;
                isUpdated = true;
            }

            return isUpdated;
        }
        public static bool FillD2(ref TblMasMember myMember, CvnImpDelegate myDelegate)
        {
            string UpdatedBy = "Delegate Import API";
            bool isUpdated = false;
            bool isNew = false;
            if (myMember == null)
            {
                myMember = new TblMasMember();
                isNew = true;
            }

            // if myMember is null that means that we are adding a new one so we need to
            // spin up a new model object and skip memberid assignmane
            // if we have an exitsting incomning member recored it's MemberID will already
            // be set so don't mess with it
            // myMember.MemberId = ??

            myMember.KofCid = (int)myDelegate.D2MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            // COUNCIL
            if (myMember.Council != (int)myDelegate.CouncilNumber)
            {
                myMember.Council = (int)myDelegate.CouncilNumber;
                myMember.CouncilUpdated = DateTime.Now;
                myMember.CouncilUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // FIRSTNAME
            if (ShouldUpdate(myDelegate.D2FirstName, myMember.FirstName, myMember, "FirstName", isNew))
            {
                myMember.FirstName = myDelegate.D2FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (ShouldUpdate(myDelegate.D2LastName, myMember.LastName, myMember, "LastName", isNew))
            {
                myMember.LastName = myDelegate.D2LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (ShouldUpdate(myDelegate.D2MiddleName, myMember.MI, myMember, "MiddleName", isNew))
            {
                myMember.MI = myDelegate.D2MiddleName;
                myMember.MIUpdated = DateTime.Now;
                myMember.MIUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (ShouldUpdate(myDelegate.D2Suffix, myMember.Suffix, myMember, "Suffix", isNew))
            {
                myMember.Suffix = myDelegate.D2Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (ShouldUpdate(myDelegate.D2Address1, myMember.Address, myMember, "Address", isNew))
            {
                myMember.Address = myDelegate.D2Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddressUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (ShouldUpdate(myDelegate.D2City, myMember.City, myMember, "City", isNew))
            {
                myMember.City = myDelegate.D2City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (ShouldUpdate(GetStateAbbr(myDelegate.D2State), myMember.State, myMember, "State", isNew))
            {
                myMember.State = GetStateAbbr(myDelegate.D2State);
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (ShouldUpdate(myDelegate.D2ZipCode, myMember.PostalCode, myMember, "PostalCode", isNew))
            {
                myMember.PostalCode = myDelegate.D2ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (ShouldUpdate(myDelegate.D2Phone, myMember.Phone, myMember, "Phone", isNew))
            {
                myMember.Phone = myDelegate.D2Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (ShouldUpdate(myDelegate.D2Email, myMember.Email, myMember, "Email", isNew))
            {
                myMember.Email = myDelegate.D2Email;
                myMember.EmailUpdated = DateTime.Now;
                myMember.EmailUpdatedBy = UpdatedBy;
                isUpdated = true;
            }

            return isUpdated;
        }
        public static bool FillA1(ref TblMasMember myMember, CvnImpDelegate myDelegate)
        {
            string UpdatedBy = "Delegate Import API";
            bool isUpdated = false;
            bool isNew = false;
            if (myMember == null)
            {
                myMember = new TblMasMember();
                isNew = true;
            }

            // if myMember is null that means that we are adding a new one so we need to
            // spin up a new model object and skip memberid assignmane
            // if we have an exitsting incomning member recored it's MemberID will already
            // be set so don't mess with it
            // myMember.MemberId = ??

            myMember.KofCid = (int)myDelegate.A1MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            // COUNCIL
            if (myMember.Council != (int)myDelegate.CouncilNumber)
            {
                myMember.Council = (int)myDelegate.CouncilNumber;
                myMember.CouncilUpdated = DateTime.Now;
                myMember.CouncilUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // FIRSTNAME
            if (ShouldUpdate(myDelegate.A1FirstName, myMember.FirstName, myMember, "FirstName", isNew))
            {
                myMember.FirstName = myDelegate.A1FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (ShouldUpdate(myDelegate.A1LastName, myMember.LastName, myMember, "LastName", isNew))
            {
                myMember.LastName = myDelegate.A1LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (ShouldUpdate(myDelegate.A1MiddleName, myMember.MI, myMember, "MiddleName", isNew))
            {
                myMember.MI = myDelegate.A1MiddleName;
                myMember.MIUpdated = DateTime.Now;
                myMember.MIUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (ShouldUpdate(myDelegate.A1Suffix, myMember.Suffix, myMember, "Suffix", isNew))
            {
                myMember.Suffix = myDelegate.A1Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (ShouldUpdate(myDelegate.A1Address1, myMember.Address, myMember, "Address", isNew))
            {
                myMember.Address = myDelegate.A1Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddressUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (ShouldUpdate(myDelegate.A1City, myMember.City, myMember, "City", isNew))
            {
                myMember.City = myDelegate.A1City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (ShouldUpdate(GetStateAbbr(myDelegate.A1State), myMember.State, myMember, "State", isNew))
            {
                myMember.State = GetStateAbbr(myDelegate.A1State);
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (ShouldUpdate(myDelegate.A1ZipCode, myMember.PostalCode, myMember, "PostalCode", isNew))
            {
                myMember.PostalCode = myDelegate.A1ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (ShouldUpdate(myDelegate.A1Phone, myMember.Phone, myMember, "Phone", isNew))
            {
                myMember.Phone = myDelegate.A1Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (ShouldUpdate(myDelegate.A1Email, myMember.Email, myMember, "Email", isNew))
            {
                myMember.Email = myDelegate.A1Email;
                myMember.EmailUpdated = DateTime.Now;
                myMember.EmailUpdatedBy = UpdatedBy;
                isUpdated = true;
            }

            return isUpdated;
        }
        public static bool FillA2(ref TblMasMember myMember, CvnImpDelegate myDelegate)
        {
            string UpdatedBy = "Delegate Import API";
            bool isUpdated = false;
            bool isNew = false;
            if (myMember == null)
            {
                myMember = new TblMasMember();
                isNew = true;
            }

            // if myMember is null that means that we are adding a new one so we need to
            // spin up a new model object and skip memberid assignmane
            // if we have an exitsting incomning member recored it's MemberID will already
            // be set so don't mess with it
            // myMember.MemberId = ??

            myMember.KofCid = (int)myDelegate.A2MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            // COUNCIL
            if (myMember.Council != (int)myDelegate.CouncilNumber)
            {
                myMember.Council = (int)myDelegate.CouncilNumber;
                myMember.CouncilUpdated = DateTime.Now;
                myMember.CouncilUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // FIRSTNAME
            if (ShouldUpdate(myDelegate.A2FirstName, myMember.FirstName, myMember, "FirstName", isNew))
            {
                myMember.FirstName = myDelegate.A2FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (ShouldUpdate(myDelegate.A2LastName, myMember.LastName, myMember, "LastName", isNew))
            {
                myMember.LastName = myDelegate.A2LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (ShouldUpdate(myDelegate.A2MiddleName, myMember.MI, myMember, "MiddleName", isNew))
            {
                myMember.MI = myDelegate.A2MiddleName;
                myMember.MIUpdated = DateTime.Now;
                myMember.MIUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (ShouldUpdate(myDelegate.A2Suffix, myMember.Suffix, myMember, "Suffix", isNew))
            {
                myMember.Suffix = myDelegate.A2Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (ShouldUpdate(myDelegate.A2Address1, myMember.Address, myMember, "Address", isNew))
            {
                myMember.Address = myDelegate.A2Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddressUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (ShouldUpdate(myDelegate.A2City, myMember.City, myMember, "City", isNew))
            {
                myMember.City = myDelegate.A2City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (ShouldUpdate(GetStateAbbr(myDelegate.A2State), myMember.State, myMember, "State", isNew))
            {
                myMember.State = GetStateAbbr(myDelegate.A2State);
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (ShouldUpdate(myDelegate.A2ZipCode, myMember.PostalCode, myMember, "PostalCode", isNew))
            {
                myMember.PostalCode = myDelegate.A2ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (ShouldUpdate(myDelegate.A2Phone, myMember.Phone, myMember, "Phone", isNew))
            {
                myMember.Phone = myDelegate.A2Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (ShouldUpdate(myDelegate.A2Email, myMember.Email, myMember, "Email", isNew))
            {
                myMember.Email = myDelegate.A2Email;
                myMember.EmailUpdated = DateTime.Now;
                myMember.EmailUpdatedBy = UpdatedBy;
                isUpdated = true;
            }

            return isUpdated;
        }

    }
}
