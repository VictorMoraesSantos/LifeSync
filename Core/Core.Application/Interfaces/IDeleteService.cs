using BuildingBlocks.Results;

namespace Core.Application.Interfaces
{
    public interface IDeleteService<TDelete>
    {
        Task<Result<bool>> DeleteAsync(TDelete dto, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteRangeAsync(IEnumerable<TDelete> dtos, CancellationToken cancellationToken = default);
    }
}
