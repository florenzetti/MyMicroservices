using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyMicroservices.Customers.Models;

namespace MyMicroservices.Customers.Infrastructure.Configurations
{
    public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Name).IsRequired();
            builder.Property(o => o.Address).IsRequired();
            builder.Property(o => o.DateOfBirth).IsRequired();


            builder.Property(o => o.CreationDate).ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasMany(o => o.CreditCards);

            builder.ToTable("Customers");
        }
    }
}
