﻿using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Features.DailyProgress.Queries.GetAll
{
    public record GetDailyProgressesQuery() : IQuery<GetDailyProgressesResult>;
    public record GetDailyProgressesResult(IEnumerable<DailyProgressDTO> DailyProgresses);
}
