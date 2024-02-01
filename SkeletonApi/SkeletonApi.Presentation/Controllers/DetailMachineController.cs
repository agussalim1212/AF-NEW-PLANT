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
        public async Task<ActionResult<Result<GetAllDetailMachineEnergyConsumptionDto>>> GetAllEnergyConsumption(Guid machineId, string type, DateTime startTime, DateTime endTime)
        {
            return await _mediator.Send(new GetAllDetailMachineEnergyConsumptionQuery(machineId, type, startTime, endTime));
        }

        [HttpGet("air-consumption")]
        public async Task<ActionResult<Result<GetAllDetailMachineAirAndElectricConsumptionDto>>> GetAllAirConsumption(Guid machineId, string type, DateTime startTime, DateTime endTime)
        {
            string view = "air_consumption_setting";
            return await _mediator.Send(new GetAllDetailMachineAirAndElectricConsumptionQuery(view, machineId, type, startTime, endTime));
        }
        
        [HttpGet("electric-consumption")]
        public async Task<ActionResult<Result<GetAllDetailMachineAirAndElectricConsumptionDto>>> GetAllElectricConsumption(Guid machineId, string type, DateTime startTime, DateTime endTime)
        {
            string view = "electric_consumption_setting";
            return await _mediator.Send(new GetAllDetailMachineAirAndElectricConsumptionQuery(view, machineId, type, startTime, endTime));
        }
    }
}
