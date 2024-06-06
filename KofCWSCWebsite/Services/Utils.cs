using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

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
            
    }
}
