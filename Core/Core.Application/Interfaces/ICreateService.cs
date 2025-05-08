namespace Core.Application.Interfaces
{
    public interface ICreateService<TCreate>
    {
        Task<bool> CreateAsync(TCreate dto, CancellationToken cancellationToken = default);
        Task<bool> CreateRangeAsync(IEnumerable<TCreate> dto, CancellationToken cancellationToken = default);
    }
}
