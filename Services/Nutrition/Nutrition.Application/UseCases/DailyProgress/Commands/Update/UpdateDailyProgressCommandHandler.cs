using MediatR;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.DailyProgress.Commands.Update
{
    public class UpdateDailyProgressCommandHandler : IRequestHandler<UpdateDailyProgressCommand, UpdateDailyProgressResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public UpdateDailyProgressCommandHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<UpdateDailyProgressResult> Handle(UpdateDailyProgressCommand command, CancellationToken cancellationToken)
        {
            UpdateDailyProgressDTO dto = new(
                command.Id,
                command.CaloriesConsumed,
                command.LiquidsConsumedMl);

            bool result = await _dailyProgressService.UpdateAsync(dto, cancellationToken);
            UpdateDailyProgressResult response = new(result);
            return response;
        }
    }
}
