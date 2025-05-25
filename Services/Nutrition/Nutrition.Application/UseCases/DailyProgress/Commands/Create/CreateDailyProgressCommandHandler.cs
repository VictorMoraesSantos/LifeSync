using MediatR;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.DailyProgress.Commands.Create
{
    public class CreateDailyProgressCommandHandler : IRequestHandler<CreateDailyProgressCommand, CreateDailyProgressResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public CreateDailyProgressCommandHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<CreateDailyProgressResult> Handle(CreateDailyProgressCommand command, CancellationToken cancellationToken)
        {
            CreateDailyProgressDTO dto = new(
                command.UserId,
                command.Date,
                command.CaloriesConsumed,
                command.LiquidsConsumedMl);

            int result = await _dailyProgressService.CreateAsync(dto, cancellationToken);
            return new CreateDailyProgressResult(result);
        }
    }
}
