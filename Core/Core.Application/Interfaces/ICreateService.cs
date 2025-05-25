namespace Core.Application.Interfaces
{
    public interface ICreateService<TCreate>
    {
        Task<int> CreateAsync(TCreate dto, CancellationToken cancellationToken = default);
        Task<IEnumerable<int>> CreateRangeAsync(IEnumerable<TCreate> dto, CancellationToken cancellationToken = default);
    }
}
