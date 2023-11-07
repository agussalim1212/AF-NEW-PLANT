using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.EnergyConsumptionAssyUnitLine;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.TotalProduction;
using SkeletonApi.Shared;


namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/assy-unit-line")]
    public class AssyUnitLineController
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public AssyUnitLineController(IMediator mediator, ILogger<AssyUnitLineController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


    [HttpGet("energy-consumption")]
    public async Task<ActionResult<Result<GetAllEnergyConsumptionDto>>> GetEnergyConsumption(Guid machineId, string type, DateTime start, DateTime end)
    {
        return await _mediator.Send(new GetAllEnergyConsumptionQuery(machineId, type, start, end));
    }

    
    [HttpGet("total-production")]
    public async Task<ActionResult<Result<GetAllTotalProductionDto>>> GetTotalProduction(Guid machineId, string type, DateTime start, DateTime end)
    {
        return await _mediator.Send(new GetAllTotalProductionQuery(machineId, type, start, end));
    }

    }

}
