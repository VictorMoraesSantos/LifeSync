using BuildingBlocks.Results;
using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.Features.Diary.Commands.Create;
using Nutrition.Application.Features.Diary.Commands.Delete;
using Nutrition.Application.Features.Diary.Commands.Update;
using Nutrition.Application.Features.Diary.Queries.Get;
using Nutrition.Application.Features.Diary.Queries.GetAll;
using Nutrition.Application.Features.Diary.Queries.GetByUser;

namespace Nutrition.API.Controllers
{
    public class DiariesController : ApiController
    {
        private readonly IMediator _mediator;

        public DiariesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetDiaryResult>> Get(int id)
        {
            GetDiaryQuery query = new(id);
            GetDiaryResult result = await _mediator.Send(query);
            return HttpResult<GetDiaryResult>.Ok(result);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<GetAllDiariesByUserIdResult>> GetByUserId(int userId)
        {
            GetAllDiariesByUserIdQuery query = new(userId);
            GetAllDiariesByUserIdResult result = await _mediator.Send(query);
            return HttpResult<GetAllDiariesByUserIdResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetDiariesResult>> GetAll([FromQuery] GetDiariesQuery query)
        {
            GetDiariesResult result = await _mediator.Send(query);
            return HttpResult<GetDiariesResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateDiaryResult>> Create([FromBody] CreateDiaryCommand command)
        {
            var result = await _mediator.Send(command);
            return HttpResult<CreateDiaryResult>.Created(result);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateDiaryResult>> Update(int id, [FromBody] UpdateDiaryCommand command)
        {
            UpdateDiaryCommand updateDiary = new(id, command.Date);
            UpdateDiaryResult result = await _mediator.Send(updateDiary);
            return HttpResult<UpdateDiaryResult>.Updated();
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteDiaryResult>> Delete(int id)
        {
            DeleteDiaryCommand command = new(id);
            DeleteDiaryResult result = await _mediator.Send(command);
            return HttpResult<DeleteDiaryResult>.Deleted();
        }
    }
}
