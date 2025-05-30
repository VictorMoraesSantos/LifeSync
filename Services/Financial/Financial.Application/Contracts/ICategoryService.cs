using Core.Application.Interfaces;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Contracts
{
    public interface ICategoryService
        : IReadService<CategoryDTO, int>,
        ICreateService<CreateCategoryDTO>,
        IUpdateService<UpdateCategoryDTO>,
        IDeleteService<int>
    {
        Task<IEnumerable<CategoryDTO?>> GetByNameAsync(string name, int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDTO?>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }
}
