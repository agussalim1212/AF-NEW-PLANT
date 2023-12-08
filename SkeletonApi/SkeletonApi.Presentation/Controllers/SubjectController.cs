using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.Machines.Queries.GetAllMachines;
using SkeletonApi.Application.Features.Machines.Queries.GetMachinesWithPagination;
using SkeletonApi.Application.Features.SubjectHasMachines.Commands.CreateSubjectHasMachine;
using SkeletonApi.Application.Features.SubjectHasMachines.Commands.DeleteSubjectHasMachine;
using SkeletonApi.Application.Features.SubjectHasMachines.Commands.UpdateSubjectHasMachine_;
using SkeletonApi.Application.Features.SubjectHasMachines.Queries.GetSubjectMachineWithPagination;
using SkeletonApi.Application.Features.SubjectHasMachines.Queries.GetSubjectWithParam;
using SkeletonApi.Application.Features.Subjects.Commands.CreateSubject;
using SkeletonApi.Application.Features.Subjects.Commands.DeleteSubject;
using SkeletonApi.Application.Features.Subjects.Commands.UpdateSubject;
using SkeletonApi.Application.Features.Subjects.Queries.GetAllSubject;
using SkeletonApi.Application.Features.Subjects.Queries.GetSubjectById;
using SkeletonApi.Application.Features.Subjects.Queries.GetSubjectWithPagination;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("api/subject")]
    public class SubjectController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;

        public SubjectController(IMediator mediator, ILogger<SubjectController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpGet("get-subject-all")]
        public async Task<ActionResult<PaginatedResult<GetSubjectWithPaginationDto>>> GetMachinesWithPagination([FromQuery] GetSubjectWithPaginationQuery query)
        {
            var validator = new GetSubjectWithPaginationValidator();

            // Call Validate or ValidateAsync and pass the object which needs to be validated
            //Response.Headers.Add("x-pagination", JsonSerializer.Serialize(pagedResult.metaData));
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

        [HttpGet("get-list-subject")]
        public async Task<ActionResult<Result<List<GetAllSubjectDto>>>> GetAll()
        {
            return await _mediator.Send(new GetAllSubjectQuery());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Result<GetSubjectByIdDto>>> GetById(Guid id)
        {
            return await _mediator.Send(new GetSubjectByIdQuery(id));
        }

        [HttpPost("create-subject")]
        public async Task<ActionResult<Result<Subject>>> Create(CreateSubjectCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result<Subject>>> Update(Guid id, UpdateSubjectCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result<Guid>>> Delete(Guid id)
        {
            return await _mediator.Send(new DeleteSubjectCommand(id));
        }

        [HttpPost("create-subject-machine")]
        public async Task<ActionResult<Result<SubjectHasMachine>>> CreateSubjectMachine(CreateSubjectHasMachineCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("update-subject-machine/{id}")]
        public async Task<ActionResult<Result<SubjectHasMachine>>> UpdateSubjectMachine(Guid id, UpdateSubjectHasMachinesCommand command)
        {
            if (id != command.MachineId)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }

        [HttpDelete("delete-subject-machine/{id}")]
        public async Task<ActionResult<Result<Guid>>> DeleteSubject(Guid id)
        {
            return await _mediator.Send(new DeleteSubjectHasMachinesCommand(id));
        }

        [HttpGet("get-all-subject-machine")]
        public async Task<ActionResult<PaginatedResult<GetSubjectMachineWithPaginationDto>>> GetSubjectMachinesWithPagination([FromQuery] GetSubjectMachineWithPaginationQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetSubjectMachineWithPaginationValidator();

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
        [HttpGet("get-subject-with-param")]
        public async Task<ActionResult<Result<List<GetSubjectWithParamDto>>>> GetSubjectWithParam(Guid machine_id)
        {
            return await _mediator.Send(new GetSubjectWithParamQuery(machine_id));
        }
    }
}