﻿using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.Routine.Commands.UpdateRoutineCommand
{
    public record UpdateRoutineCommand(
        int Id,
        string Name,
        string Description)
        : ICommand<UpdateRoutineResponse>;
    public record UpdateRoutineResponse(bool IsSuccess);
}
