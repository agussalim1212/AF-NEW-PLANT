using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.DTOs.AirAndElectricConsumption;
using SkeletonApi.Application.DTOs.CurrentAndVoltageConsumption;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.CurrentConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.ElectricCosumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.MachineInformation;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.TotalProduction;
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
        public async Task<ActionResult<Result<GetAllDetailMachineAirAndElectricConsumptionDto>>> GetAllAirConsumption(Guid machineId, string type, DateTime start, DateTime end)
        {
            //nama materialized view untuk air consumption
            string nameView = "air_consumption_setting";
            //vid yang mengandung air consumption
            string vid = "AIR-CONSUMPTION";
            return await _mediator.Send(new GetAllDetailMachineAirConsumptionQuery(machineId, type, start, end, nameView, vid));
        }

        [HttpGet("electric-consumption")]
        public async Task<ActionResult<Result<GetAllDetailMachineAirAndElectricConsumptionDto>>> GetAllElectricConsumption(Guid machineId, string type, DateTime start, DateTime end)
        {
            //nama materialized view untuk electric consumption
            string nameView = "electric_consumption_setting";
            //vid yang mengandung electric consumption
            string vid = "ELECT-GNTR";
            return await _mediator.Send(new GetAllDetailMachineElectricConsumptionQuery(machineId, type, start, end, nameView, vid));
        }

        [HttpGet("current-consumption")]
        public async Task<ActionResult<Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>>> GetAllCurrentConsumption(Guid machineId, string type, DateTime start, DateTime end)
        {
            //vid yang mengandung current consumption
            string vid = "CURRENT";
            //nama materialized view untuk current consumption
            string nameView = "current_consumption";
            return await _mediator.Send(new GetAllDetailMachineCurrentConsumptionQuery(machineId, type, start, end, vid, nameView));
        }

        [HttpGet("voltage-consumption")]
        public async Task<ActionResult<Result<GetAllDetailMachineCurrentAndVoltageConsumptionDto>>> GetAllVoltageConsumption(Guid machineId, string type, DateTime start, DateTime end)
        {
            //vid yang mengandung voltage consumption
            string vid = "VOLTAGE";
            //nama materialized view untuk voltage consumption
            string nameView = "voltage_consumption";
            return await _mediator.Send(new GetAllDetailMachineVoltageConsumptionQuery(machineId, type, start, end, vid, nameView));
        }

        [HttpGet("machine-information")]
        public async Task<ActionResult<Result<GetAllMachineInformationDto>>> GetMachineInformation(Guid machine_id)
        {
            return await _mediator.Send(new GetAllMachineInformationQuery(machine_id));
        }

        [HttpGet("production")]
        public async Task<ActionResult<Result<GetAllTotalProductionDto>>> GetAllProduction(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllTotalProductionQuery(machine_id, type, start, end));
        }
    }
}