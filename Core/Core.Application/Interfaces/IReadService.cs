using BuildingBlocks.Results;
using Core.Application.DTO;
using System.Linq.Expressions;

namespace Core.Application.Interfaces
{
    public interface IReadService<TRead, TId> where TRead : DTOBase
    {
        Task<Result<TRead?>> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<TRead?>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<(IEnumerable<TRead?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<TRead?>>> FindAsync(Expression<Func<TRead, bool>> predicate, CancellationToken cancellationToken = default);
        Task<Result<int>> CountAsync(Expression<Func<TRead, bool>>? predicate = null, CancellationToken cancellationToken = default);
    }
}
