using Financial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financial.Infrastructure.Configuration
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.OwnsOne(o => o.Amount, amountBuilder =>
            {
                amountBuilder.Property(m => m.Amount);
                amountBuilder.Property(m => m.Currency);
            });
        }
    }
}
