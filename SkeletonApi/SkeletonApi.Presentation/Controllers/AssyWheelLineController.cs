using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.AirConsumptionAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.EnergyConsumptionAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelFrontWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.MachineInformationAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.TotalProductionAssyWheelLine;
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
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    if (query.type_wheel == "final_inspection")
                    {

                        worksheet.Cell(1, 1).Value = "date_time";
                        worksheet.Cell(1, 2).Value = "status";
                        worksheet.Cell(1, 3).Value = "data_dial_horizontal";
                        worksheet.Cell(1, 4).Value = "data_dial_vertical";

                        for (int i = 0; i < pg.Data.Count(); i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                            worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).Status;
                            worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DataDialHorizontal;
                            worksheet.Cell(i + 2, 4).Value = pg.Data.ElementAt(i).DataDialVertical;
                        }
                    }
                    else if (query.type_wheel == "tire_inflation")
                    {
                        worksheet.Cell(1, 1).Value = "date_time";
                        worksheet.Cell(1, 2).Value = "tire_inflation";


                        for (int i = 0; i < pg.Data.Count(); i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                            worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).TirePresure;
                        }
                    }
                    else if (query.type_wheel == "disk_brake")
                    {
                        worksheet.Cell(1, 1).Value = "date_time";
                        worksheet.Cell(1, 2).Value = "data_torQ";

                        for (int i = 0; i < pg.Data.Count(); i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                            worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).DataTorQ;
                        }
                    }
                    else
                    {
                        {
                            worksheet.Cell(1, 1).Value = "date_time";
                            worksheet.Cell(1, 2).Value = "status";
                            worksheet.Cell(1, 3).Value = "data_distance";
                            worksheet.Cell(1, 4).Value = "data_tonase";


                            for (int i = 0; i < pg.Data.Count(); i++)
                            {
                                worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                                worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).Status;
                                worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DataDistance;
                                worksheet.Cell(i + 2, 4).Value = pg.Data.ElementAt(i).DataTonase;

                            }
                        }
                    }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        try
                        {
                            if(query.type_wheel == null)
                            {
                                query.type_wheel = "Press Bearing";
                            }
                            var fileName = $"List_Quality_{query.type_wheel}_{DateTime.Now.ToString("yyyy-MMMM-dddd")}.xlsx";
                            Response.Headers.Add("x-download", $"List_Quality_{query.type_wheel}_{DateTime.Now.ToString("yyyy-MMMM-dddd")}.xlsx");

                            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException == null)
                            {
                                return Problem(ex.Message);
                            }
                            return Problem(ex.InnerException.Message);
                        }
                    }
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
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    if (query.type_wheel == "final_inspection")
                    {

                        worksheet.Cell(1, 1).Value = "date_time";
                        worksheet.Cell(1, 2).Value = "status";
                        worksheet.Cell(1, 3).Value = "data_dial_horizontal";
                        worksheet.Cell(1, 4).Value = "data_dial_vertical";

                        for (int i = 0; i < pg.Data.Count(); i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                            worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).Status;
                            worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DataDialHorizontal;
                            worksheet.Cell(i + 2, 4).Value = pg.Data.ElementAt(i).DataDialVertical;
                        }
                    }
                    else if (query.type_wheel == "tire_inflation")
                    {
                        worksheet.Cell(1, 1).Value = "date_time";
                        worksheet.Cell(1, 2).Value = "tire_inflation";


                        for (int i = 0; i < pg.Data.Count(); i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                            worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).TirePresure;
                        }
                    }
                    else if (query.type_wheel == "disk_brake")
                    {
                        worksheet.Cell(1, 1).Value = "date_time";
                        worksheet.Cell(1, 2).Value = "data_torQ";

                        for (int i = 0; i < pg.Data.Count(); i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                            worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).DataTorQ;
                        }
                    }
                    else
                    {
                        {
                            worksheet.Cell(1, 1).Value = "date_time";
                            worksheet.Cell(1, 2).Value = "status";
                            worksheet.Cell(1, 3).Value = "data_distance";
                            worksheet.Cell(1, 4).Value = "data_tonase";


                            for (int i = 0; i < pg.Data.Count(); i++)
                            {
                                worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                                worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).Status;
                                worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DataDistance;
                                worksheet.Cell(i + 2, 4).Value = pg.Data.ElementAt(i).DataTonase;

                            }
                        }
                    }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        try
                        {
                            if (query.type_wheel == null)
                            {
                                query.type_wheel = "Press Bearing";
                            }
                            var fileName = $"List_Quality_{query.type_wheel}_{DateTime.Now.ToString("yyyy-MMMM-dddd")}.xlsx";
                            Response.Headers.Add("x-download", $"List_Quality_{query.type_wheel}_{DateTime.Now.ToString("yyyy-MMMM-dddd")}.xlsx");

                            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException == null)
                            {
                                return Problem(ex.Message);
                            }
                            return Problem(ex.InnerException.Message);
                        }
                    }
                }

            }
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            return BadRequest(errorMessages);
        }
        #endregion Excel Wheel Rear



    }

}
