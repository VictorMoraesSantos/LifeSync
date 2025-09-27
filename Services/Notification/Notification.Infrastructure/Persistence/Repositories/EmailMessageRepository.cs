using EmailSender.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Persistence.Data;
using System.Linq.Expressions;
namespace Notification.Infrastructure.Persistence.Repositories
{
    public class EmailMessageRepository : IEmailMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public EmailMessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(EmailMessage entity, CancellationToken cancellationToken = default)
        {
            await _context.EmailMessages.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<EmailMessage> entities, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(EmailMessage entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<EmailMessage?>> Find(Expression<Func<EmailMessage, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<EmailMessage?> emailMessages = await _context.EmailMessages
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return emailMessages;
        }

        public async Task<IEnumerable<EmailMessage?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<EmailMessage> emailMessages = await _context.EmailMessages
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return emailMessages;
        }

        public async Task<EmailMessage?> GetById(int id, CancellationToken cancellationToken = default)
        {
            EmailMessage? emailMessage = await _context.EmailMessages
                .FirstOrDefaultAsync(em => em.Id == id, cancellationToken);

            return emailMessage;
        }

        public Task Update(EmailMessage entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}