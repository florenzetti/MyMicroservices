using Microsoft.EntityFrameworkCore;
using MyMicroservices.Customers.Infrastructure.Configurations;
using MyMicroservices.Customers.Models;

namespace MyMicroservices.Customers.Infrastructure
{
    public class CustomersContext : DbContext
    {
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CreditCard> CreditCards { get; set; }

        public CustomersContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CreditCardEntityConfiguration());
        }
    }
}
