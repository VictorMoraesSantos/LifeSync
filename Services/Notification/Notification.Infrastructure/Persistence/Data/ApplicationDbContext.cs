using BuildingBlocks.Results;
using EmailSender.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Notification.Infrastructure.Persistence.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<EmailMessage> EmailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Error>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
