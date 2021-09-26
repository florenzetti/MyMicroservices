using System;

namespace MyMicroservices.Customers.Models
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
