using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.AirConsumption;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.TotalProduction;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.AirConsumptionAssyWheel;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.EnergyConsumptionAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.MachineInformation;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/assy-wheel-line")]
    public class AssyWheelLineController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public AssyWheelLineController(IMediator mediator, ILogger<AssyWheelLineController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpGet("energy-consumption")]
        public async Task<ActionResult<Result<GetAllEnergyConsumptionAssyWheelLineDto>>> GetEnergyConsumption(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllEnergyConsumptionAssyWheelLineQuery(machine_id, type, start, end));
        }

        [HttpGet("total-production")]
        public async Task<ActionResult<Result<GetAllTotalProductionDto>>> GetTotalProduction(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllTotalProductionQuery(machine_id, type, start, end));
        }

        [HttpGet("air-consumption")]
        public async Task<ActionResult<Result<GetAllAirConsumptionAssyWheelDto>>> GetAirConsumptionAssyWheel(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllAirConsumptionAssyWheelQuery(machine_id, type, start, end));
        }
        [HttpGet("machine-information")]
        public async Task<ActionResult<Result<GetAllMachineInformationAssyWheelLineDto>>> GetMachineInformationAssyWheelLine(Guid machine_id)
        {
            return await _mediator.Send(new GetAllMachineInformationAssyWheelLineQuery(machine_id));
        }


    }

}
