using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
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
        private readonly ISender _sender;

        public DiariesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetDiaryResult>> Get(int id)
        {
            GetDiaryQuery query = new(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetDiaryResult>.Ok(result.Value!)
                : HttpResult<GetDiaryResult>.NotFound(result.Error!.Description);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<GetAllDiariesByUserIdResult>> GetByUserId(int userId)
        {
            GetAllDiariesByUserIdQuery query = new(userId);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetAllDiariesByUserIdResult>.Ok(result.Value!)
                : HttpResult<GetAllDiariesByUserIdResult>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<GetDiariesResult>> GetAll([FromQuery] GetDiariesQuery query)
        {
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetDiariesResult>.Ok(result.Value!)
                : HttpResult<GetDiariesResult>.InternalError(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<CreateDiaryResult>> Create([FromBody] CreateDiaryCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<CreateDiaryResult>.Created(result.Value!)
                : HttpResult<CreateDiaryResult>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateDiaryResult>> Update(int id, [FromBody] UpdateDiaryCommand command)
        {
            UpdateDiaryCommand updateDiary = new(id, command.Date);
            var result = await _sender.Send(updateDiary);

            if (result.IsSuccess)
                return HttpResult<UpdateDiaryResult>.Ok(result.Value!);

            if (result.Error!.Description.Contains("NotFound"))
                return HttpResult<UpdateDiaryResult>.NotFound(result.Error!.Description);

            return HttpResult<UpdateDiaryResult>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteDiaryResult>> Delete(int id)
        {
            DeleteDiaryCommand command = new(id);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<DeleteDiaryResult>.Deleted()
                : HttpResult<DeleteDiaryResult>.NotFound(result.Error!.Description);
        }
    }
}