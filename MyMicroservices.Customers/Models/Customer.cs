using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyMicroservices.Customers.Models
{
    public class Customer : Entity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        public string Salt { get; set; }

        public virtual ICollection<CreditCard> CreditCards { get; set; }
    }
}
