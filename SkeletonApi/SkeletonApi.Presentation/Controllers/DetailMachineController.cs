using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using SkeletonApi.Shared;


namespace SkeletonApi.Presentation.Controllers
{
    [Route("api/detail-machine")]
    public class DetailMachineController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public DetailMachineController(IMediator mediator, ILogger<DetailMachineController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("energy-consumption")]
        public async Task<ActionResult<Result<GetAllDetailMachineEnergyConsumptionDto>>> GetAllEnergyConsumption(Guid machineId, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllDetailMachineEnergyConsumptionQuery(machineId, type, start, end));
        }

        [HttpGet("air-consumption")]
        public async Task<ActionResult<Result<GetAllDetailMachineAirAndElectricConsumptionDto>>> GetAllAirConsumption(Guid machineId, string type, DateTime start, DateTime end)
        {
            string view = "air_consumption_setting";
            string vid = "AIR-CONSUMPTION";
            return await _mediator.Send(new GetAllDetailMachineAirAndElectricConsumptionQuery(machineId, type, start, end, view, vid));
        }
        
        [HttpGet("electric-consumption")]
        public async Task<ActionResult<Result<GetAllDetailMachineAirAndElectricConsumptionDto>>> GetAllElectricConsumption(Guid machineId, string type, DateTime start, DateTime end)
        {
            string view = "electric_consumption_setting";
            string vid = "ELECT-GNTR";
            return await _mediator.Send(new GetAllDetailMachineAirAndElectricConsumptionQuery(machineId, type, start, end, view, vid));
        }
    }
}
