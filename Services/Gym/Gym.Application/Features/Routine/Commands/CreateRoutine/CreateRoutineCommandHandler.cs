﻿using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.Routine;

namespace Gym.Application.Features.Routine.Commands.CreateRoutine
{
    public class CreateRoutineCommandHandler : ICommandHandler<CreateRoutineCommand, CreateRoutineResponse>
    {
        private readonly IRoutineService _routineService;

        public CreateRoutineCommandHandler(IRoutineService routineService)
        {
            _routineService = routineService;
        }

        public async Task<Result<CreateRoutineResponse>> Handle(CreateRoutineCommand command, CancellationToken cancellationToken)
        {
            var dto = new CreateRoutineDTO(
                command.UserId,
                command.Name,
                command.Description);

            var result = await _routineService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateRoutineResponse>(result.Error!);

            return Result.Success(new CreateRoutineResponse(result.Value!));
        }
    }
}
