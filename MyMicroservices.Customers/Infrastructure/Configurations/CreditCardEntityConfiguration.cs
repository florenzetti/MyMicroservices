using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyMicroservices.Customers.Models;

namespace MyMicroservices.Customers.Infrastructure.Configurations
{
    public class CreditCardEntityConfiguration : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure(EntityTypeBuilder<CreditCard> builder)
        {
            builder.HasKey(o => o.Id);

            builder.ToTable("CreditCards");
        }
    }
}
