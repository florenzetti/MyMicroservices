using System;
using System.Collections.Generic;

namespace MyMicroservices.Customers.Models
{
    public class Customer : Entity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<CreditCard> CreditCards { get; set; }
    }
}
