using System;
using System.ComponentModel.DataAnnotations;

namespace MyMicroservices.Customers.Dto
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
