using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.EnergyConsumptionGensubAssyLine;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.ListQualityGensub.ListQualityGensubWithPagination;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.MachineInformation;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.TotalProduction;
using SkeletonApi.Shared;
using System.Text.Json;


namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/gensub-assy-line")]
    public class GensubAssyLineController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public GensubAssyLineController(IMediator mediator, ILogger<GensubAssyLineController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("energy-consumption")]
        public async Task<ActionResult<Result<GetAllEnergyConsumptionGensubDto>>> GetEnergyConsumptionGensub(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllEnergyConsumptionGensubQuery(machine_id, type, start, end));
        }

        [HttpGet("machine-information")]
        public async Task<ActionResult<Result<GetAllMachineInformationGensubDto>>> GetMachineInformationGensub(Guid machine_id)
        {
            return await _mediator.Send(new GetAllMachineInformationGensubQuery(machine_id));
        }

        [HttpGet("total-production")]
        public async Task<ActionResult<Result<GetAllTotalProductionGensubDto>>> GetTotalProductionGensub(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllTotalProductionGensubQuery(machine_id, type, start, end));
        }

        [HttpGet("list-quality")]
        public async Task<ActionResult<PaginatedResult<GetListQualityGensubDto>>> GetListQualityWithPagination([FromQuery] GetListQualityGensubQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetListQualityGensubValidator();

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

    }
}



