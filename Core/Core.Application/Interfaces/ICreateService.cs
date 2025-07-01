using BuildingBlocks.Results;

namespace Core.Application.Interfaces
{
    public interface ICreateService<TCreate>
    {
        Task<Result<int>> CreateAsync(TCreate dto, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<TCreate> dto, CancellationToken cancellationToken = default);
    }
}
