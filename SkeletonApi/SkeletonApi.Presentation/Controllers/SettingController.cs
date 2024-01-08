using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.Settings;
using SkeletonApi.Application.Features.Settings.Commands.CreateSetting;
using SkeletonApi.Application.Features.Settings.Queries.GetSettingWithPagination;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json;


namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/setting")]
    public class SettingController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public SettingController(IMediator mediator, ILogger<SettingController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("list-setting")]
        public async Task<ActionResult<PaginatedResult<GetSettingWithPaginationDto>>> GetSettingWithPagination([FromQuery] GetSettingWithPaginationQuery query)
        {
            var validator = new GetSettingWithPaginationValidator();

            var result = validator.Validate(query);

            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                var paginationData = new
                {
                    pg.page_number,
                    pg.total_pages,
                    pg.page_size,
                    pg.total_count,
                    pg.has_previous,
                    pg.has_next
                };
                Response.Headers.Add("x-pagination", JsonSerializer.Serialize(paginationData));
                return Ok(pg);
            }

            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            return BadRequest(errorMessages);
        }

        [HttpPost("create-setting")]
        public async Task<ActionResult<Result<CreateSettingResponseDto>>> CreateSetting(CreateSettingRequest command)
        {
            return await _mediator.Send(command);
        }

    }
}
