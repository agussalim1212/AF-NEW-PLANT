using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.FrameNumb;
using SkeletonApi.Application.Features.FrameNumb.Commands.CreateFrameNumber;
using SkeletonApi.Application.Features.FrameNumb.Commands.DeleteFrameNumber;
using SkeletonApi.Application.Features.FrameNumb.Commands.UpdateFrameNumber;
using SkeletonApi.Application.Features.FrameNumb.Queries.GetFrameNumberWithPagination;
using SkeletonApi.Application.Features.FrameNumberHasSubject.Commands.CreateFrameNumberHasSubject;
using SkeletonApi.Application.Features.FrameNumberSubject.Commands.DeleteFrameNumberHasSubject;
using SkeletonApi.Application.Features.FrameNumberSubject.Commands.UpdateFrameNumberHasSubject;
using SkeletonApi.Application.Features.FrameNumberSubject.Queries.GetAllFrameNumber;
using SkeletonApi.Application.Features.FrameNumberSubject.Queries.GetFrameNumberHasSubjectWithPagination;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/frame-number")]
    public class FrameNumberController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public FrameNumberController(IMediator mediator, ILogger<FrameNumberController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<CreateFrameNumberResponseDto>>> Create(CreateFrameNumberRequest command)
        {
            return await _mediator.Send(command);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result<Guid>>> Delete(Guid id)
        {
            return await _mediator.Send(new DeleteFrameNumberRequest(id));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result<FrameNumber>>> Update(Guid id, UpdateFrameNumberRequest command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }

        [HttpGet("list-frame-number")]
        public async Task<ActionResult<PaginatedResult<GetFrameNumberWithPaginationDto>>> GetCategoryMachinesWithPagination([FromQuery] GetFrameNumberWithPaginationQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetFrameNumberWithPaginationValidator();

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

        [HttpPost("create-frame-subject")]
        public async Task<ActionResult<Result<FrameNumberHasSubjects>>> CreateFrameNumberHasSubject(CreateNumberHasSubjectCommand command)
        {
                return await _mediator.Send(command);
        }

        [HttpGet("get-all-frame-number")]
        public async Task<ActionResult<Result<List<GetAllFrameNumberDto>>>> GetAll()
        {
            return await _mediator.Send(new GetAllFrameNumberQuery());
        }


        [HttpGet("list-frame-number-subject")]
        public async Task<ActionResult<PaginatedResult<GetFrameNumberWithPaginationDto>>> GetFrameNumberHasSubjectWithPagination([FromQuery] GetFrameNumberHasSubjectWithPaginationQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetFrameNumberHasSubjectWithPaginationValidator();

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

        [HttpDelete("delete-frame-number-subject/{id:guid}")]
        public async Task<ActionResult<Result<Guid>>> DeleteFrameNumberHasSubject(Guid id)
        {
            return await _mediator.Send(new DeleteFrameNumberHasSubjectCommand(id));
        }

        [HttpPut("update-frame-number-subject/{id}")]
        public async Task<ActionResult<Result<FrameNumberHasSubjects>>> UpdateFrameNumberHasSubject(Guid id, UpdateFrameNumberHasSubjectCommand command)
        {
            if (id != command.FrameNumberId)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }

    }


}
