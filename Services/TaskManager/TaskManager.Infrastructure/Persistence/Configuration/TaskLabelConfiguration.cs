using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configuration
{
    public class TaskLabelConfiguration : IEntityTypeConfiguration<TaskLabel>
    {
        public void Configure(EntityTypeBuilder<TaskLabel> builder)
        {
            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(t => t.LabelColor)
                .IsRequired();
                
            builder.Property(t => t.UserId)
                .IsRequired();
                
            builder.HasIndex(t => t.UserId);
        }
    }
}
