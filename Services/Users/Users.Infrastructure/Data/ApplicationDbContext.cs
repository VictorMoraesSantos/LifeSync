using BuildingBlocks.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;

namespace Users.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Ignore<Error>();

            builder.Entity<User>(b =>
            {
                b.OwnsOne(u => u.Name, n =>
                {
                    n.Property(p => p.FirstName).HasColumnName("FirstName").IsRequired();
                    n.Property(p => p.LastName).HasColumnName("LastName").IsRequired();
                    n.Ignore(p => p.FullName); // Propriedade calculada, não mapeada
                });

                b.OwnsOne(u => u.Contact, c =>
                {
                    c.Property(p => p.Email).HasColumnName("Email");
                });

                // Outras configurações do User...
            });
        }
    }
}
