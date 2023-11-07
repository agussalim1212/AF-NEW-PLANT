using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.CategoryMachine.Commands.CreateCategoryHasMachine;
using SkeletonApi.Application.Features.CategoryMachine.Commands.UpdateCategoryHasMachine;
using SkeletonApi.Application.Features.CategoryMachine.Queries.GetAllCategoryMachine;
using SkeletonApi.Application.Features.CategoryMachine.Queries.GetCategoryMachinesWithPagination;
using SkeletonApi.Application.Features.CategoryMachine.Queries.GetCategoryMachineWithPagination;
using SkeletonApi.Application.Features.CategoryMachine.Queries.GetCategoryMachineWithPagination_;
using SkeletonApi.Application.Features.Machines.Commands.CreateMachines;
using SkeletonApi.Application.Features.Machines.Commands.DeleteMachines;
using SkeletonApi.Application.Features.Machines.Commands.UpdateMachines;
using SkeletonApi.Application.Features.Machines.Queries.GetAllMachines;
using SkeletonApi.Application.Features.Machines.Queries.GetMachinesWithPagination;
using SkeletonApi.Application.Features.Subjects.Queries.GetAllSubject;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/machine")]

    public class MachineController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;

        public MachineController(IMediator mediator, ILogger<MachineController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        //[HttpGet]
        //public async Task<ActionResult<Result<List<GetAllMachinesDto>>>> Get()
        //{
        //    _logger.LogInformation("Here is info message from our values controller.");
        //    _logger.LogDebug("Here is debug message from our values controller.");
        //    _logger.LogWarning("Here is warn message from our values controller.");
        //    _logger.LogError("Here is an error message from our values controller.");
        //    //throw new Exception();
        //    //return Ok("OK");
        //    return await _mediator.Send(new GetAllMachinesQuery());
        //}


        [HttpGet("list-machine")]
        public async Task<ActionResult<PaginatedResult<GetMachinesWithPaginationDto>>> GetMachinesWithPagination([FromQuery] GetMachinesWithPaginationQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetMachinesWithPaginationValidator();

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

        [HttpGet("get-all-machine")]
        public async Task<ActionResult<Result<List<GetAllMachinesDto>>>> GetAll()
        {
            return await _mediator.Send(new GetAllMachinesQuery());
        }
        [HttpGet("get-all-category-machine-has-machine")]
        public async Task<ActionResult<PaginatedResult<GetCategoryMachinesWithPaginationDto>>> GetCategoryMachinesWithPagination([FromQuery] GetCategoryMachinesWithPaginationQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetCategoryMachineWithPaginationValidator();

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

        [HttpGet("get-list-category-machine")]
        public async Task<ActionResult<Result<List<GetAllCategoryMachineDto>>>> GetAllCategory()
        {
            return await _mediator.Send(new GetAllCategoryMachineQuery());
        }

        [HttpPost("create-category-machine")]
        public async Task<ActionResult<Result<CategoryMachineHasMachine>>> CreateCategoryMachine(CreateCategoryHasMachineCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("update-category-machine/{id}")]
        public async Task<ActionResult<Result<CategoryMachineHasMachine>>> UpdateCategory(Guid id,UpdateCategoryHasMachinesCommand command)
        {
            if (id != command.CategoryMachineId)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }

        [HttpPost]
        public async Task<ActionResult<Result<Machine>>> Create(CreateMachinesCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result<Guid>>> Delete(Guid id)
        {
            return await _mediator.Send(new DeleteMachinesCommand(id));
        }


        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result<Machine>>> Update(Guid id, UpdateMachinesCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }


    }
}
