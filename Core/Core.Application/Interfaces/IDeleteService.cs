namespace Core.Application.Interfaces
{
    public interface IDeleteService<TDelete>
    {
        Task<bool> DeleteAsync(TDelete dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteRangeAsync(IEnumerable<TDelete> dtos, CancellationToken cancellationToken = default);
    }
}
