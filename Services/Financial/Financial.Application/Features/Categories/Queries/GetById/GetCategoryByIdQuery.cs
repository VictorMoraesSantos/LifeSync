using BuildingBlocks.CQRS.Queries;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Features.Categories.Queries.GetById
{
    public record GetCategoryByIdQuery(int Id) : IQuery<GetCategoryByIdResult>;
    public record GetCategoryByIdResult(CategoryDTO Category);
}
