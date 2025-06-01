using BuildingBlocks.CQRS.Request;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Features.Categories.Queries.GetAll
{
    public record GetAllCategoriesQuery() : IRequest<GetAllCategoriesResult>;
    public record GetAllCategoriesResult(IEnumerable<CategoryDTO> Categories);
}
