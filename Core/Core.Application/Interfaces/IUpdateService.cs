using BuildingBlocks.Results;

namespace Core.Application.Interfaces
{
    public interface IUpdateService<TUpdate>
    {
        Task<Result<bool>> UpdateAsync(TUpdate dto, CancellationToken cancellationToken = default);
    }
}
