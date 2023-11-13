using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.AirConsumptionAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.EnergyConsumptionAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.MachineInformationAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.TotalProductionAssyWheelLine;
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
        public async Task<ActionResult<Result<GetAllEnergyConsumptionAssyWheelLineDto>>> GetEnergyConsumptionAssyWheelLine(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllEnergyConsumptionAssyWheelLineQuery(machine_id, type, start, end));
        }

        [HttpGet("total-production")]
        public async Task<ActionResult<Result<GetAllTotalProductionAssyWheelLineDto>>> GetTotalProductionAssyWheelLine(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllTotalProductionAssyWheelLineQuery(machine_id, type, start, end));
        }

        [HttpGet("machine-information")]
        public async Task<ActionResult<Result<GetAllMachineInformationAssyWheelLineDto>>> GetMachineInformationAssyWheelLine(Guid machine_id)
        {
            return await _mediator.Send(new GetAllMachineInformationAssyWheelLineQuery(machine_id));
        }

        [HttpGet("air-consumption")]
        public async Task<ActionResult<Result<GetAllAirConsumptionAssyWheelLineDto>>> GetAirConsumptionAssyWheelLine(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllAirConsumptionAssyWheelLineQuery(machine_id, type, start, end));
        }




    }

}
