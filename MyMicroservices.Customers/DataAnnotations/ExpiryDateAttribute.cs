using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MyMicroservices.Customers.DataAnnotations
{
    public class ExpiryDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string stringValue = value as string;
            if (stringValue is null)
            {
                return false;
            }

            var match = Regex.Match(stringValue, @"^(\d{2})\/(\d{4})$");
            if (!match.Success)
            {
                return false;
            }

            var month = int.Parse(match.Groups[1].Value);
            if (month <= 0 || month > 12)
            {
                return false;
            }

            var year = int.Parse(match.Groups[2].Value);
            if (year <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
