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
            builder.Property(o => o.Type).IsRequired();
            builder.Property(o => o.CardNumberHash).IsRequired();
            builder.Property(o => o.ExpiryDate).HasMaxLength(7).IsRequired();
            builder.Property(o => o.CVVHash).IsRequired();
            builder.Property(o => o.Salt).IsRequired();

            builder.Property(o => o.CreationDate).ValueGeneratedOnAdd().HasDefaultValueSql("getutcdate()");

            builder.ToTable("CreditCards");
        }
    }
}
