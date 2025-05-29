using Core.Domain.Repositories;
using Financial.Domain.Entities;

namespace Financial.Domain.Repositories
{
    public interface IFinancialAccountRepository : IRepository<FinancialAccount, int>
    {
        Task<IEnumerable<FinancialAccount>> GetAllByUserIdAsync(int userId);
    }
}
