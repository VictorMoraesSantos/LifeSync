﻿using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Diary.Commands.Update
{
    public class UpdateDiaryCommandHandler : IRequestHandler<UpdateDiaryCommand, UpdateDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public UpdateDiaryCommandHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<UpdateDiaryResult> Handle(UpdateDiaryCommand command, CancellationToken cancellationToken)
        {
            UpdateDiaryDTO dto = new(command.Id, command.Date);

            bool result = await _diaryService.UpdateAsync(dto, cancellationToken);

            return new UpdateDiaryResult(result);
        }
    }
}
