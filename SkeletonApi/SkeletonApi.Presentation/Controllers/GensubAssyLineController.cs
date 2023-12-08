using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.EnergyConsumptionGensubAssyLine;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.ListQualityGensub.ListQualityAutoTighteningFrontCoshionWithPagination;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.ListQualityGensub.ListQualityGensubWithPagination;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.MachineInformation;
using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.TotalProduction;
using SkeletonApi.Shared;
using System.Text.Json;


namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/gensub-assy-line")]
    public class GensubAssyLineController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public GensubAssyLineController(IMediator mediator, ILogger<GensubAssyLineController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("energy-consumption")]
        public async Task<ActionResult<Result<GetAllEnergyConsumptionGensubDto>>> GetEnergyConsumptionGensub(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllEnergyConsumptionGensubQuery(machine_id, type, start, end));
        }

        [HttpGet("machine-information")]
        public async Task<ActionResult<Result<GetAllMachineInformationGensubDto>>> GetMachineInformationGensub(Guid machine_id)
        {
            return await _mediator.Send(new GetAllMachineInformationGensubQuery(machine_id));
        }

        [HttpGet("total-production")]
        public async Task<ActionResult<Result<GetAllTotalProductionGensubDto>>> GetTotalProductionGensub(Guid machine_id, string type, DateTime start, DateTime end)
        {
            return await _mediator.Send(new GetAllTotalProductionGensubQuery(machine_id, type, start, end));
        }

        [HttpGet("list-quality")]
        public async Task<ActionResult<PaginatedResult<GetListQualityGensubDto>>> GetListQualityWithPagination([FromQuery] GetListQualityGensubQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetListQualityGensubValidator();

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

        #region Excel List Quality Auto Cover Body Assy And Conv Steering Handle
        [HttpGet("list-quality-download")]
        public async Task<ActionResult<PaginatedResult<GetListQualityGensubDto>>> ListQualityDownloadExcel([FromQuery] GetListQualityGensubQuery query)
        {
            var validator = new GetListQualityGensubValidator();

            var result = validator.Validate(query);
            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    worksheet.Cell(1, 1).Value = "date_time";
                    worksheet.Cell(1, 2).Value = "status";
    
                    for (int i = 0; i < pg.Data.Count(); i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                        worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).Status;

                    }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        try
                        {
                            var fileName = $"List_Quality_{DateTime.Now.ToString("yyyy-MMMM-dddd")}.xlsx";
                            Response.Headers.Add("x-download", $"List_Quality_{DateTime.Now.ToString("yyyy-MMMM-dddd")}.xlsx");

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

        #endregion Excel List Quality Auto Cover Body Assy And Conv Steering Handle

        [HttpGet("list-quality-auto-tightening-fc")]
        public async Task<ActionResult<PaginatedResult<GetListQualityAutoTIghteningFcDto>>> GetListQualityAutoTighteningFcWithPagination([FromQuery] GetListQualityAutoTIghteningFcQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetListQualityAutoTIghteningFcValidator();

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

        #region Excel List Quality Auto Tightening Front Cushion
        [HttpGet("list-quality-download-auto-tightening")]
        public async Task<ActionResult<PaginatedResult<GetListQualityAutoTIghteningFcDto>>> ListQualityAutoTighteningFCDownloadExcel([FromQuery] GetListQualityAutoTIghteningFcQuery query)
        {
            var validator = new GetListQualityAutoTIghteningFcValidator();

            var result = validator.Validate(query);
            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    worksheet.Cell(1, 1).Value = "date_time";
                    worksheet.Cell(1, 2).Value = "status";
                    worksheet.Cell(1, 3).Value = "data_barcode";
                    worksheet.Cell(1, 4).Value = "data_torsi";

                    for (int i = 0; i < pg.Data.Count(); i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                        worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).Status;
                        worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DataBarcode;
                        worksheet.Cell(i + 2, 4).Value = pg.Data.ElementAt(i).DataTorQ;

                    }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        try
                        {
                            var fileName = $"List_Quality_{DateTime.Now.ToString("yyyy-MMMM-dddd")}.xlsx";
                            Response.Headers.Add("x-download", $"List_Quality_{DateTime.Now.ToString("yyyy-MMMM-dddd")}.xlsx");

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

        #endregion Excel List Quality Auto Tightening Front Cushion

    }
}



