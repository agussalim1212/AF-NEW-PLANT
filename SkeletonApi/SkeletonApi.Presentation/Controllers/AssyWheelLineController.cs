using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.AirConsumptionAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.EnergyConsumptionAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelFrontWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.MachineInformationAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.TotalProductionAssyWheelLine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelFrontWithPagination.Download;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination.Download;
using SkeletonApi.Shared;
using System.Text.Json;


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

        [HttpGet("list-quality-wheel-front")]
        public async Task<ActionResult<PaginatedResult<GetListWheelFrontDto>>> GetListQualityWheelFrontWithPagination([FromQuery] GetListQualityWheelFrontQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetListQualityWheelFrontValidator();

            var result = validator.Validate(query);

            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                var paginationData = new
                {
                    pg.page_number,
                    pg.total_pages,
                    pg.page_size,
                    pg.total_count,
                    pg.has_previous,
                    pg.has_next
                };

                Response.Headers.Add("x-pagination", JsonSerializer.Serialize(paginationData));
                return Ok(pg);
            }

            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            return BadRequest(errorMessages);
        }

        #region Excel Wheel Front
        [HttpGet("list-quality-download-wheel-front")]
        public async Task<ActionResult<PaginatedResult<GetListWheelFrontDto>>> DownloadExcelWheelFront([FromQuery] GetListQualityWheelFrontQuery query)
        {
            var validator = new GetListQualityWheelFrontValidator();

            var result = validator.Validate(query);
            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                byte[]? bytes = null;
                string filename = string.Empty;
                string types = query.type_wheel;
                try
                {
                    if (pg != null)
                    {
                        var Export = new DownloadListQualityWheelFrontToExcel(pg);
                        Export.GetListExcel(ref bytes, ref types, ref filename);
                    }
                    Response.Headers.Add("x-download", filename);
                    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null) { return Problem(ex.Message); }
                    return Problem(ex.InnerException.Message);
                }

            }
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            return BadRequest(errorMessages);
        }
        #endregion Excel Wheel Front

        [HttpGet("list-quality-wheel-rear")]
        public async Task<ActionResult<PaginatedResult<GetListWheelRearDto>>> GetListQualityWheelRearWithPagination([FromQuery] GetListWheelRearQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetListWheelRearValidator();

            var result = validator.Validate(query);

            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                var paginationData = new
                {
                    pg.page_number,
                    pg.total_pages,
                    pg.page_size,
                    pg.total_count,
                    pg.has_previous,
                    pg.has_next
                };

                Response.Headers.Add("x-pagination", JsonSerializer.Serialize(paginationData));
                return Ok(pg);
            }

            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            return BadRequest(errorMessages);
        }

        #region Excel Wheel Rear
        [HttpGet("list-quality-download-wheel-rear")]
        public async Task<ActionResult<PaginatedResult<GetListWheelRearDto>>> DownloadExcelWheelRear([FromQuery] GetListWheelRearQuery query)
        {
            var validator = new GetListWheelRearValidator();
            var result = validator.Validate(query);
            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                byte[]? bytes = null;
                string filename = string.Empty;
                string types = query.type_wheel;
                try
                {
                    if (pg != null)
                    {
                        var Export = new DownloadListQualityWheelRearToExcel(pg);
                        Export.GetListExcel(ref bytes, ref types, ref filename);
                    }
                    Response.Headers.Add("x-download", filename);
                    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null) { return Problem(ex.Message); }
                    return Problem(ex.InnerException.Message);
                }
            }

            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            return BadRequest(errorMessages);
        }
        #endregion Excel Wheel Rear



    }

}
