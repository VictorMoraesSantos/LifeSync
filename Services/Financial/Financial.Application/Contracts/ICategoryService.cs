using BuildingBlocks.Results;
using Core.Application.Interfaces;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Contracts
{
    public interface ICategoryService
        : IReadService<CategoryDTO, int, CategoryFilterDTO>,
        ICreateService<CreateCategoryDTO>,
        IUpdateService<UpdateCategoryDTO>,
        IDeleteService<int>
    {
        Task<Result<IEnumerable<CategoryDTO?>>> GetByNameAsync(string name, int userId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<CategoryDTO?>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }
}
