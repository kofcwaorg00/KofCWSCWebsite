using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace KofCWSCWebsite.Data
{
    

    public class NoPoBoxAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var address = value.ToString();
                if (Regex.IsMatch(address, @"P\.?\s*O\.?\s*Box", RegexOptions.IgnoreCase))
                {
                    return new ValidationResult("PO Box addresses are not allowed.");
                }
            }
            return ValidationResult.Success;
        }
    }

}
