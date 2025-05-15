using BuildingBlocks.Results;
using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.DTOs.Diaries;
using Nutrition.Application.UseCases.Diary.Commands.Create;
using Nutrition.Application.UseCases.Diary.Commands.Delete;
using Nutrition.Application.UseCases.Diary.Commands.Update;
using Nutrition.Application.UseCases.Diary.Queries.Get;
using Nutrition.Application.UseCases.Diary.Queries.GetAll;
using Nutrition.Application.UseCases.Diary.Queries.GetByUser;

namespace Nutrition.API.Controllers
{
    public class DiaryController : ApiController
    {
        private readonly IMediator _mediator;

        public DiaryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<HttpResult<GetDiaryQueryResult>> Get(int id)
        {
            GetDiaryQuery query = new(id);
            var result = await _mediator.Send(query);
            return HttpResult<GetDiaryQueryResult>.Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<HttpResult<GetAllDiariesByUserIdResult>> GetByUserId(int userId)
        {
            GetAllDiariesByUserIdQuery query = new(userId);
            var result = await _mediator.Send(query);
            return HttpResult<GetAllDiariesByUserIdResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetDiariesQueryResult>> GetAll([FromQuery] GetDiariesQuery query)
        {
            var result = await _mediator.Send(query);
            return HttpResult<GetDiariesQueryResult>.Ok(result);
        }

        [HttpPost("create")]
        public async Task<HttpResult<CreateDiaryCommandResult>> Create([FromBody] CreateDiaryCommand command)
        {
            var result = await _mediator.Send(command);
            return HttpResult<CreateDiaryCommandResult>.Created(result);
        }

        [HttpPut]
        public async Task<HttpResult<UpdateDiaryCommandResult>> Update([FromBody] UpdateDiaryCommand command)
        {
            var result = await _mediator.Send(command);
            return HttpResult<UpdateDiaryCommandResult>.Updated();
        }

        [HttpDelete("{id}")]
        public async Task<HttpResult<DeleteDiaryCommandResult>> Delete(int id)
        {
            DeleteDiaryCommand command = new(id);
            var result = await _mediator.Send(command);
            return HttpResult<DeleteDiaryCommandResult>.Deleted();
        }
    }
}
