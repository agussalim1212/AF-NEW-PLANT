using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AmpereConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.ElectricCosumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.MachineInformation;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.VoltageConsumptionDetailMachine;
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
        public async Task<ActionResult<Result<GetAllDetailMachineAirConsumptionDto>>> GetAllAirConsumption(Guid machineId, string type, DateTime start, DateTime end)
        {
            string view = "air_consumption_setting";
            string vid = "AIR-CONSUMPTION";
            return await _mediator.Send(new GetAllDetailMachineAirConsumptionQuery(machineId, type, start, end, view, vid));
        }
        
        [HttpGet("electric-consumption")]
        public async Task<ActionResult<Result<GetAllDetailMachineAirConsumptionDto>>> GetAllElectricConsumption(Guid machineId, string type, DateTime start, DateTime end)
        {
            string view = "electric_consumption_setting";
            string vid = "ELECT-GNTR";
            return await _mediator.Send(new GetAllDetailMachineElectricConsumptionQuery(machineId, type, start, end, view, vid));
        }

        [HttpGet("current-consumption")]
        public async Task<ActionResult<Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>>> GetAllCurrentConsumption(Guid machineId, string type, DateTime start, DateTime end)
        {
            string vid = "CURRENT";
            string view = "current_consumption";
            return await _mediator.Send(new GetAllDetailMachineCurrentConsumptionQuery(machineId, type, start, end, vid, view));
        }

        [HttpGet("voltage-consumption")]
        public async Task<ActionResult<Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>>> GetAllVoltageConsumption(Guid machineId, string type, DateTime start, DateTime end)
        {
            string vid = "VOLTAGE";
            string view = "voltage_consumption";
            return await _mediator.Send(new GetAllDetailMachineVoltageConsumptionQuery(machineId, type, start, end, vid, view));
        }

        [HttpGet("machine-information")]
        public async Task<ActionResult<Result<GetAllMachineInformationDto>>> GetMachineInformation(Guid machine_id)
        {
            return await _mediator.Send(new GetAllMachineInformationQuery(machine_id));
        }
    }
}
