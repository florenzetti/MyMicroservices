using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMicroservices.Customers.Models
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
