using FinancialControl.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financial.Infrastructure.Configuration
{
    public class MoneyConfiguration : IEntityTypeConfiguration<Money>
    {
        public void Configure(EntityTypeBuilder<Money> builder)
        {
            builder.HasNoKey();
        }
    }
}
