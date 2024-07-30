using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.Subjects.Queries.GetSubjectWithPagination;
using SkeletonApi.Application.Features.Treacibility.Queries.GetDetailTreacibility;
using SkeletonApi.Application.Features.Treacibility.Queries.GetTreacibiltyWithPagination;
using SkeletonApi.Shared;
using System.Text.Json;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("api/traceability")]
    public class TreacibilityController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;

        public TreacibilityController(IMediator mediator, ILogger<TreacibilityController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("get-traceability-all")]
        public async Task<ActionResult<PaginatedResult<GetTreacibilityWithPaginationDto>>> GetTreacibilityWithPagination([FromQuery] GetTreacibilityWithPaginationQuery query)
        {
            var validator = new GetTreacibiltyWithPaginationValidator();

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

        [HttpGet("get-traceability-detail")]
        public async Task<ActionResult<Result<GetTreacibilityDetailDto>>> GetAllTreacibility(string engine_id, string torsi)
        {
            return await _mediator.Send(new GetTreacibilityDetailQuery(engine_id, torsi));
        }
    }
}