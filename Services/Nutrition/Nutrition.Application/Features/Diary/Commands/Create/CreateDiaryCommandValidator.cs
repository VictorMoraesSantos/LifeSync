using FluentValidation;

namespace Nutrition.Application.Features.Diary.Commands.Create
{
    public class CreateDiaryCommandValidator : AbstractValidator<CreateDiaryCommand>
    {
        public CreateDiaryCommandValidator()
        {
            RuleFor(command => command.Date)
                .NotEmpty().WithMessage("A data é obrigatória.");
        }
    }
}