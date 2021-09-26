using System;

namespace MyMicroservices.Customers.Models
{
    public enum CreditCardType
    {
        Amex,
        Visa,
        MasterCard
    }

    public class CreditCard : Entity
    {
        public CreditCardType Type { get; set; }
        public string CardNumberHash { get; set; }
        public string ExpiryDate { get; set; }
        public string CVVHash { get; set; }
        public string Salt { get; set; }

        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
