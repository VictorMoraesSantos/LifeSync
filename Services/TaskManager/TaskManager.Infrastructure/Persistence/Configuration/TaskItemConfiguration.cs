using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configuration
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            // Configuração de tabela
            builder.ToTable("TaskItems");

            // Chave primária
            builder.HasKey(t => t.Id);

            // Propriedades
            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.Priority)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.DueDate)
                .IsRequired();

            builder.Property(t => t.UserId)
                .IsRequired();

            // Configuração do Soft Delete
            builder.Property(t => t.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Configuração do relacionamento com TaskLabel
            builder.HasMany(t => t.Labels)
               .WithOne(l => l.TaskItem)
               .HasForeignKey(l => l.TaskItemId)
               .OnDelete(DeleteBehavior.SetNull); // CHANGE THIS LINE

            // Índices para otimização de consultas
            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.DueDate);
            builder.HasIndex(t => t.IsDeleted);
        }
    }
}