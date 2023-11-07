using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.EnergyConsumptionGensubAssyLine;
using SkeletonApi.Shared;


namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/gensub-assy-line")]
    public class GensubAssyLineController
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public GensubAssyLineController(IMediator mediator, ILogger<GensubAssyLineController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("energy-consumption")]
        public async Task<ActionResult<Result<GetAllEnergyConsumptionGensubDto>>> GetEnergyConsumptionGensub(Guid machineId, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllEnergyConsumptionGensubQuery(machineId, type, start, end));
        }

    }
}



