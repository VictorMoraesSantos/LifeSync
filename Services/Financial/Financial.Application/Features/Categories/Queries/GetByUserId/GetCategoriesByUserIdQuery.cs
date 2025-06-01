using BuildingBlocks.CQRS.Request;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Features.Categories.Queries.GetByUserId
{
    public record GetCategoriesByUserIdQuery(int UserId) : IRequest<GetCategoriesByUserIdResult>;
    public record GetCategoriesByUserIdResult(IEnumerable<CategoryDTO> Categories);
}
