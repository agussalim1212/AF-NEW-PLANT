using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.MaintenanceCorrective.Commands.Create;
using SkeletonApi.Application.Features.MaintenanceCorrective.Commands.Delete;
using SkeletonApi.Application.Features.MaintenanceCorrective.Commands.Update;
using SkeletonApi.Application.Features.MaintenanceCorrective.Queries.GetAll;
using SkeletonApi.Application.Features.MaintenanceCorrective.Queries.GetDetail;
using SkeletonApi.Application.Features.MaintenanceCorrective.Queries.GetListWithPagination;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/maintenance-corrective")]
    public class MaintCorrectiveController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;

        public MaintCorrectiveController(IMediator mediator, ILogger<MaintCorrective> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("create-maintenance-corrective")]
        public async Task<ActionResult<Result<CreateMaintCorrectiveDto>>> Create([FromBody] CreateMaintCorrectiveCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<Result<UpdateMaintCorrectiveDto>>> Update(Guid id, [FromBody] UpdateMaintCorrectiveCommand command)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()) || string.IsNullOrWhiteSpace(command.Id.ToString()))
            {
                return BadRequest();
            }

            if (id != command.Id)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result<Guid>>> Delete(Guid id)
        {
            return await _mediator.Send(new DeleteMaintCorrectiveCommand(id));
        }

        [HttpGet("list-maintenance-corrective")]
        public async Task<ActionResult<PaginatedResult<GetListMaintCorrectiveWithPaginationDto>>> GetListMaintCorrectiveWithPagination([FromQuery] GetListMaintCorrectiveWithPaginationQuery query)
        {
            var validator = new GetListMaintCorrectiveWithPaginationValidator();

            // Call Validate or ValidateAsync and pass the object which needs to be validated
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

        [HttpGet("list-detail/{id:guid}")]
        public async Task<ActionResult<Result<List<GetDetailMaintCorrectiveDto>>>> GetDetail(Guid id)
        {
            return await _mediator.Send(new GetDetailMaintCorrectiveQuery(id));
        }

        [HttpGet("maintenance-corrective-chart")]
        public async Task<ActionResult<Result<List<GetAllMaintCorrectiveDto>>>> GetAllMaintenance(DateTime? start_date, DateTime? end_date)
        {
            return await _mediator.Send(new GetAllMaintCorrectiveQuery(start_date, end_date));
        }
    }
}
