using BuildingBlocks.Results;

namespace Core.Application.Interfaces
{
    public interface IDeleteService<TId>
    {
        Task<Result<bool>> DeleteAsync(TId dto, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteRangeAsync(IEnumerable<TId> dtos, CancellationToken cancellationToken = default);
    }
}
