using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.AirConsumption;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ElectricGeneratorConsumption;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.EnergyConsumptionAssyUnitLine;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.FrequencyInverter;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityAssyUnitLineWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFiling;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityMainLine;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityNutRunnerSteeringStemWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityOilBrake;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityPressConeRace;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityRobotScanImage;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.MachineInformation;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.StopLine;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.TotalProduction;
using SkeletonApi.Shared;
using System.Text.Json;

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

        [HttpGet("list-quality-nut-runner")]
        public async Task<ActionResult<PaginatedResult<GetListQualityNutRunnerSteeringStemDto>>> GetListQualaityNutRunnerSteringStem([FromQuery] GetListQualityNutRunnerSteeringStemQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetListQualityNutRunnerSteeringStemValidator();

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

    #region Excel Nut Runner Steering Stem And Rear Wheel
        [HttpGet("list-quality-download-nut-runner")]
        public async Task<ActionResult<PaginatedResult<GetListQualityNutRunnerSteeringStemDto>>> DownloadExcelNutRunner([FromQuery] GetListQualityNutRunnerSteeringStemQuery query)
        {
            var validator = new GetListQualityNutRunnerSteeringStemValidator();

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
                    worksheet.Cell(1, 3).Value = "data_torsi";


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
        #endregion Excel Nut Runner Steering Stem And Rear Wheel

        [HttpGet("list-quality-robot-and-abs-tester")]
        public async Task<ActionResult<PaginatedResult<GetListQualityRobotScanImageDto>>> GetListQualityRobotScanImageAndAbsTester([FromQuery] GetListQualityRobotScanImageQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetListQualityRobotScanImageValidator();

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

        #region Excel Robot And Abs Tester

        [HttpGet("list-quality-download-robot-and-abs-tester")]
        public async Task<ActionResult<PaginatedResult<GetListQualityRobotScanImageDto>>> DownloadExcelScanImageAndAbsTester([FromQuery] GetListQualityRobotScanImageQuery query)
        {        
            var validator = new GetListQualityRobotScanImageValidator();

            var result = validator.Validate(query);
            if (result.IsValid)
            {
            var pg = await _mediator.Send(query);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                worksheet.Cell(1, 1).Value = "date_time";
                worksheet.Cell(1, 2).Value = "data_barcode";
                worksheet.Cell(1, 3).Value = "status";
             

                for (int i = 0; i < pg.Data.Count(); i++)
                {
                    worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                    worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).DataBarcode;
                    worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).Status;
                  
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
        #endregion Robot And Abs Tester

        [HttpGet("list-quality-main-line")]
        public async Task<ActionResult<PaginatedResult<GetListQualityMainLineDto>>> GetListQualityMainLine([FromQuery] GetListQualityMainLineQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetListQualityMainLineValidator();

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

        #region Excel Main Line

        [HttpGet("list-quality-download-main-line")]
        public async Task<ActionResult<PaginatedResult<GetListQualityMainLineDto>>> DownloadExcelMainLine([FromQuery] GetListQualityMainLineQuery query)
        {
            var validator = new GetListQualityMainLineValidator();

            var result = validator.Validate(query);
            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    worksheet.Cell(1, 1).Value = "date_time";
                    worksheet.Cell(1, 2).Value = "frq_inverter";
                    worksheet.Cell(1, 3).Value = "duration_stop";


                    for (int i = 0; i < pg.Data.Count(); i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                        worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).FrqInverter;
                        worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DurationStop;

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
        #endregion Excel Main Line

        [HttpGet("list-quality-coolant-filing")]
        public async Task<ActionResult<PaginatedResult<GetListQualityCoolantFilingDto>>> GetListQualityCoolantFiling([FromQuery] GetListQualityCoolantFilingQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetListQualityCoolantFilingValidator();

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

        #region Excel Coolant Filing
        [HttpGet("list-quality-download-coolant-filing")]
        public async Task<ActionResult<PaginatedResult<GetListQualityCoolantFilingDto>>> DownloadExcelCoolantFiling([FromQuery] GetListQualityCoolantFilingQuery query)
        {
            var validator = new GetListQualityCoolantFilingValidator();

            var result = validator.Validate(query);
            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    worksheet.Cell(1, 1).Value = "date_time";
                    worksheet.Cell(1, 2).Value = "volume_coolant";
                    worksheet.Cell(1, 3).Value = "data_barcode";


                    for (int i = 0; i < pg.Data.Count(); i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                        worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).VolumeCoolant;
                        worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DataBarcode;

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
        #endregion Excel Coolant Filing

        [HttpGet("list-quality-oil-brake")]
        public async Task<ActionResult<PaginatedResult<GetListQualityOilBrakeDto>>> GetListQualityOilBrake([FromQuery] GetListQualityOilBrakeQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetListQualityOilBrakeValidator();

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

        #region Excel Oil Brake
        [HttpGet("list-quality-download-oil-brake")]
        public async Task<ActionResult<PaginatedResult<GetListQualityOilBrakeDto>>> DownloadExcelOilBrake([FromQuery] GetListQualityOilBrakeQuery query)
        {
            var validator = new GetListQualityOilBrakeValidator();

            var result = validator.Validate(query);
            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    worksheet.Cell(1, 1).Value = "date_time";
                    worksheet.Cell(1, 2).Value = "data_barcode";
                    worksheet.Cell(1, 3).Value = "leak_tester";
                    worksheet.Cell(1, 4).Value = "volume_oil_brake";
                    worksheet.Cell(1, 5).Value = "status";
                    worksheet.Cell(1, 6).Value = "error_code";


                    for (int i = 0; i < pg.Data.Count(); i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                        worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).DataBarcode;
                        worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).LeakTester;
                        worksheet.Cell(i + 2, 4).Value = pg.Data.ElementAt(i).VolumeOilBrake;
                        worksheet.Cell(i + 2, 5).Value = pg.Data.ElementAt(i).Status;
                        worksheet.Cell(i + 2, 6).Value = pg.Data.ElementAt(i).ErrorCode;

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
        #endregion Excel Oil Brake

        [HttpGet("list-quality-press-cone-race")]
        public async Task<ActionResult<PaginatedResult<GetListQualityPressConeRaceDto>>> GetListQualityPressConeRaceBrake([FromQuery] GetListQualityPressConeRaceQuery query)
        {
            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var validator = new GetListQualityPressConeRaceValidator();

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

        #region Excel Press Cone Race
        [HttpGet("list-quality-download-press-cone-race")]
        public async Task<ActionResult<PaginatedResult<GetListQualityPressConeRaceDto>>> DownloadExcelPressConeRace([FromQuery] GetListQualityPressConeRaceQuery query)
        {
            var validator = new GetListQualityPressConeRaceValidator();

            var result = validator.Validate(query);
            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    worksheet.Cell(1, 1).Value = "date_time";
                    worksheet.Cell(1, 2).Value = "kedalaman";
                    worksheet.Cell(1, 3).Value = "tonase";


                    for (int i = 0; i < pg.Data.Count(); i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                        worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).Kedalaman;
                        worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).Tonase;

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

        #endregion Excel Press Cone Race


    }

}


