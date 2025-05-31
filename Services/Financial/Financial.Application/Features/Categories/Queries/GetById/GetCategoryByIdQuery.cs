using Financial.Application.DTOs.Category;
using MediatR;

namespace Financial.Application.Features.Categories.Queries.GetById
{
    public record GetCategoryByIdQuery(int Id) : IRequest<GetCategoryByIdResult>;
    public record GetCategoryByIdResult(CategoryDTO Category);
}
