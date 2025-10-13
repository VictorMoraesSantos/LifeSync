using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.Features.Diary.Commands.Create;
using Nutrition.Application.Features.Diary.Commands.Delete;
using Nutrition.Application.Features.Diary.Commands.Update;
using Nutrition.Application.Features.Diary.Queries.GetById;
using Nutrition.Application.Features.Diary.Queries.GetAll;
using Nutrition.Application.Features.Diary.Queries.GetByUser;

namespace Nutrition.API.Controllers
{
    public class DiariesController : ApiController
    {
        private readonly ISender _sender;

        public DiariesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> Get(int id)
        {
            GetDiaryQuery query = new(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<object>> GetByUserId(int userId)
        {
            GetAllDiariesByUserIdQuery query = new(userId);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll([FromQuery] GetDiariesQuery query)
        {
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.InternalError(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create([FromBody] CreateDiaryCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update(int id, [FromBody] UpdateDiaryCommand command)
        {
            UpdateDiaryCommand updateDiary = new(id, command.Date);
            var result = await _sender.Send(updateDiary);

            if (result.IsSuccess)
                return HttpResult<object>.Ok(result.Value!);

            if (result.Error!.Description.Contains("NotFound"))
                return HttpResult<object>.NotFound(result.Error!.Description);

            return HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id)
        {
            DeleteDiaryCommand command = new(id);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.NotFound(result.Error!.Description);
        }
    }
}