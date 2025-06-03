using Financial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financial.Infrastructure.Configuration
{
    public class FinancialAccountConfiguration : IEntityTypeConfiguration<FinancialAccount>
    {
        public void Configure(EntityTypeBuilder<FinancialAccount> builder)
        {
            builder.OwnsOne(
                o => o.Balance, balanceBuilder =>
                {
                    balanceBuilder.Property(m => m.Amount);
                    balanceBuilder.Property(m => m.Currency);
                });
        }
    }
}
