using BuildingBlocks.CQRS.Queries;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Features.Categories.Queries.GetAll
{
    public record GetAllCategoriesQuery() : IQuery<GetAllCategoriesResult>;
    public record GetAllCategoriesResult(IEnumerable<CategoryDTO> Categories);
}
