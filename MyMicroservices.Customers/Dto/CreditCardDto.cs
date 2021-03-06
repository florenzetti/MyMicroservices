using MyMicroservices.Customers.DataAnnotations;
using MyMicroservices.Customers.Models;
using System.ComponentModel.DataAnnotations;

namespace MyMicroservices.Customers.Dto
{
    public class CreditCardDto
    {
        [Required]
        public CreditCardType Type { get; set; }
        [Required, CreditCard]
        public string CardNumber { get; set; }
        [Required, ExpiryDate]
        public string ExpiryDate { get; set; }
        [Required, CVV]
        public string CVV { get; set; }
    }
}
