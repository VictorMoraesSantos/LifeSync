using BuildingBlocks.CQRS.Request;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Features.Categories.Queries.GetById
{
    public record GetCategoryByIdQuery(int Id) : IRequest<GetCategoryByIdResult>;
    public record GetCategoryByIdResult(CategoryDTO Category);
}
