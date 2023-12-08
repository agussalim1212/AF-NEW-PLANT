using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.AirConsumption;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ElectricGeneratorConsumption;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.EnergyConsumptionAssyUnitLine;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.FrequencyInverter;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.MachineInformation;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.StopLine;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.TotalProduction;
using SkeletonApi.Shared;


namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/assy-unit-line")]
    public class AssyUnitLineController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public AssyUnitLineController(IMediator mediator, ILogger<AssyUnitLineController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("energy-consumption")]
        public async Task<ActionResult<Result<GetAllEnergyConsumptionDto>>> GetEnergyConsumption(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllEnergyConsumptionQuery(machine_id, type, start, end));
        }
        [HttpGet("total-production")]
        public async Task<ActionResult<Result<GetAllTotalProductionDto>>> GetTotalProduction(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllTotalProductionQuery(machine_id, type, start, end));
        }
        [HttpGet("machine-information")]
        public async Task<ActionResult<Result<GetAllMachineInformationDto>>> GetMachineInformation(Guid machine_id)
        {
            return await _mediator.Send(new GetAllMachineInformationQuery(machine_id));
        }
        [HttpGet("air-consumption")]
        public async Task<ActionResult<Result<GetAllAirConsumptionDto>>> GetAirConsumption(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllAirConsumptionQuery(machine_id, type, start, end));
        }
        [HttpGet("stop-line")]
        public async Task<ActionResult<Result<GetAllStopLineDto>>> GetStopLine(Guid machine_id)
        {
            return await _mediator.Send(new GetAllStopLineQuery(machine_id));
        }
        [HttpGet("frequency-inverter")]
        public async Task<ActionResult<Result<GetAllFrequencyInverterDto>>> GetFrequencyInverter(Guid machine_id)
        {
            return await _mediator.Send(new GetAllFrequencyInverterQuery(machine_id));
        }
        [HttpGet("electric-generator-consumption")]
        public async Task<ActionResult<Result<GetAllElectricGeneratorConsumptionDto>>> GetElectricGenerator(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllElectricGeneratorConsumptionQuery(machine_id, type, start, end));
        }
    }

}
