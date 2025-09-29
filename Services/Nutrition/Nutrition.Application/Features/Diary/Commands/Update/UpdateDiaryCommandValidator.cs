using FluentValidation;

namespace Nutrition.Application.Features.Diary.Commands.Update
{
    public class UpdateDiaryCommandValidator : AbstractValidator<UpdateDiaryCommand>
    {
        public UpdateDiaryCommandValidator()
        {
            RuleFor(command => command.Date)
                .NotEmpty().WithMessage("A data é obrigatória.");
        }
    }
}
