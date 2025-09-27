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
    }
}
