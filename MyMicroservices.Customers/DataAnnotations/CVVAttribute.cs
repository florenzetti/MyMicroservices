using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MyMicroservices.Customers.DataAnnotations
{
    public class CVVAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var stringValue = value as string;
            if (stringValue != null && Regex.IsMatch(stringValue, @"^\d{3}$"))
            {
                return true;
            }

            return false;
        }
    }
}
