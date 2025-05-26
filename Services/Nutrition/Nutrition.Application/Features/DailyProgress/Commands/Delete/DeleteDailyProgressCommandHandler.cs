using MediatR;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.DailyProgress.Commands.Delete
{
    public class DeleteDailyProgressCommandHandler : IRequestHandler<DeleteDailyProgressCommand, DeleteDailyProgressResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public DeleteDailyProgressCommandHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<DeleteDailyProgressResult> Handle(DeleteDailyProgressCommand command, CancellationToken cancellationToken)
        {
            bool result = await _dailyProgressService.DeleteAsync(command.Id, cancellationToken);

            return new DeleteDailyProgressResult(result);
        }
    }
}
