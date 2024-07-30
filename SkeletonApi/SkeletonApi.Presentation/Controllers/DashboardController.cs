using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.Dashboard.FiveTopAirConsumption.Queries;
using SkeletonApi.Application.Features.Dashboard.FiveTopEnergyConsumption.Queries;
using SkeletonApi.Application.Features.Dashboard.FiveTopMachineMaintenance.Queries;
using SkeletonApi.Application.Features.Dashboard.TraceabilityHistory.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumption.Queries;
using SkeletonApi.Shared;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("api/dashboard")]
    public class DashboardController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;

        public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("top-energy-consumption")]
        public async Task<ActionResult<Result<GetAllTop5EnergyConsumptionsDto>>> GetAllEnergyConsumption()
        {
            return await _mediator.Send(new GetAllTop5EnergyConsumptionsQuery());
        }

        [HttpGet("top-air-consumption")]
        public async Task<ActionResult<Result<GetAllTop5AirConsumptionsDto>>> GetAllAirConsumption()
        {
            return await _mediator.Send(new GetAllTop5AirConsumptionsQuery());
        }

        [HttpGet("top-machine-maintenance")]
        public async Task<ActionResult<Result<GetAllTop5MachineMaintenanceDto>>> GetAllMachineMaintenance()
        {
            return await _mediator.Send(new GetAllTop5MachineMaintenanceQuery());
        }

        [HttpGet("detail-energy-consumption")]
        public async Task<ActionResult<Result<List<GetAllDetailEnergyConsumptionDto>>>> GetEnergyConsumption(string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllDetailEnergyConsumptionQuery(type, start, end));
        }

        [HttpGet("traceability-history")]
        public async Task<ActionResult<Result<List<GetAllTraceabilityHistoryDto>>>> GetTraceability()
        {
            return await _mediator.Send(new GetAllTraceabilityHistoryQuery());
        }
    }
}