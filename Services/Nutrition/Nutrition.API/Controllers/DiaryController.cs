using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.Diaries.Commands.CreateDiary;
using Nutrition.Application.Diaries.Commands.DeleteDiary;
using Nutrition.Application.Diaries.Commands.UpdateDiary;
using Nutrition.Application.Diaries.Queries.GetAllDiariesByUserId;
using Nutrition.Application.Diaries.Queries.GetDiaries;
using Nutrition.Application.Diaries.Queries.GetDiary;
using Nutrition.Application.DTOs.Diaries;

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
        public async Task<ActionResult<DiaryDTO>> Get(int id)
        {
            GetDiaryQuery query = new(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<DiaryDTO>> GetByUserId(int userId)
        {
            GetAllDiariesByUserIdQuery query = new(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiaryDTO>>> GetAll([FromQuery] GetDiariesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateDiaryCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDiaryCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            DeleteDiaryCommand command = new(id);
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
