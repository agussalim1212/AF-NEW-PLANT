using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.Dashboard._5_Top_Air_Consumptions.Queries;
using SkeletonApi.Application.Features.Dashboard._5_Top_Energy_Consumptions;
using SkeletonApi.Application.Features.Dashboard._5_Top_Machine_Maintenance.Queries;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.EnergyConsumptionAssyUnitLine;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions;
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
       // [ServiceFilter(typeof(AuditLoggingFilter))]
        public async Task<ActionResult<Result<GetAllTop5EnergyConsumptionsDto>>> GetAllEnergyConsumption()
        {
            return await _mediator.Send(new GetAllTop5EnergyConsumptionsQuery());
        }

        [HttpGet("top-air-consumption")]
        // [ServiceFilter(typeof(AuditLoggingFilter))]
        public async Task<ActionResult<Result<GetAllTop5AirConsumptionsDto>>> GetAllAirConsumption()
        {
            return await _mediator.Send(new GetAllTop5AirConsumptionsQuery());
        }

        [HttpGet("top-machine-maintenance")]
        // [ServiceFilter(typeof(AuditLoggingFilter))]
        public async Task<ActionResult<Result<GetAllTop5MachineMaintenanceDto>>> GetAllMachineMaintenance()
        {
            return await _mediator.Send(new GetAllTop5MachineMaintenanceQuery());
        }

        [HttpGet("detail-energy-consumption")]
        // [ServiceFilter(typeof(AuditLoggingFilter))]
        public async Task<ActionResult<Result<List<GetAllDetailEnergyConsumptionDto>>>> GetEnergyConsumption(string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllDetailEnergyConsumptionQuery(type, start, end));
        }
    }
}
