using BuildingBlocks.Results;

namespace Core.Application.Interfaces
{
    public interface IUpdateService<TDelete>
    {
        Task<Result<bool>> UpdateAsync(TDelete dto, CancellationToken cancellationToken = default);
    }
}
