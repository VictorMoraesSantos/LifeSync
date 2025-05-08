namespace Core.Application.Interfaces
{
    public interface IUpdateService<TDelete>
    {
        Task<bool> UpdateAsync(TDelete dto, CancellationToken cancellationToken = default);
    }
}
