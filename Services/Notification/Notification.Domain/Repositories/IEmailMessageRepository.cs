using Core.Domain.Repositories;
using EmailSender.Domain.Entities;

namespace Notification.Domain.Repositories
{
    public interface IEmailMessageRepository : IRepository<EmailMessage, int>
    {

    }
}
