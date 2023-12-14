using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Create;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Delete;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Update;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UpdateOK;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UploadExcel;
using SkeletonApi.Application.Features.MaintenancesPreventive.Queries.DownloadList;
using SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetAllMachine;
using SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetDetail;
using SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetListWithPagination;
using SkeletonApi.Application.Features.MaintenancesPreventive.Queries.GetMaintSchedule;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/maintenance-preventive")]

    public class MaintenancesPreventiveController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;

        public MaintenancesPreventiveController(IMediator mediator, ILogger<MaintenacePreventive> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("create-maintenance-preventive")]
        public async Task<ActionResult<Result<CreateMaintPreventiveDto>>> Create([FromBody] CreateMaintPreventiveCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("ok/{id:guid}")]
        public async Task<ActionResult<Result<UpdateMaintPreventiveOkDto>>> UpdateOK(Guid id, [FromBody] UpdateMaintPreventiveOkCommand command)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()) || string.IsNullOrWhiteSpace(command.Id.ToString()))
            {
                return BadRequest();
            }

            if (id != command.Id)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }

        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<Result<UpdateMaintPreventiveDto>>> Update(Guid id, [FromBody] UpdateMaintPreventiveCommand command)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()) || string.IsNullOrWhiteSpace(command.Id.ToString()))
            {
                return BadRequest();
            }

            if (id != command.Id)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<Guid>>> Delete(Guid id)
        {
            return await _mediator.Send(new DeleteMaintPreventiveCommand(id));
        }

        [HttpGet("get-schedule")]
        public async Task<ActionResult<Result<List<GetMaintScheduleDto>>>> GetScheduleMaintenance()
        {
            return await _mediator.Send(new GetMaintScheduleQuery());
        }

        [HttpGet("maintenance-preventive-chart")]
        public async Task<ActionResult<Result<List<GetAllMAchineMaintPreventiveDto>>>> GetAllMaintenance(DateTime? start_date, DateTime? end_date)
        {
            return await _mediator.Send(new GetAllMachineMaintPreventiveQuery(start_date, end_date));
        }

        [HttpGet("list-maintenance-preventive")]
        public async Task<ActionResult<PaginatedResult<GetListMaintPreventiveWithPaginationDto>>> GetListMaintPreventiveWithPagination([FromQuery] GetListMaintPreventiveWithPaginationQuery query)
        {
            var validator = new GetListMaintPreventiveWithPaginationValidator();

            // Call Validate or ValidateAsync and pass the object which needs to be validated
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

        [HttpGet("list-detail/{id:guid}")]
        public async Task<ActionResult<Result<List<GetDetailMaintPreventiveDto>>>> GetDetail(Guid id)
        {
            return await _mediator.Send(new GetDetailMaintPreventiveQuery(id));
        }

        [HttpGet("download-list-maintenance-preventive")]
        public async Task<ActionResult<PaginatedResult<DownloadListMaintPrevDto>>> DownloadListMaintPrevToExcel([FromQuery] DownloadListMaintPrevQuery query)
        {
            var validator = new DownloadListMaintPrevValidator();

            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var result = validator.Validate(query);

            if (result.IsValid)
            {
                var pg = await _mediator.Send(query);
                byte[]? bytes = null;
                string filename = string.Empty;
                try
                {
                    if (pg != null)
                    {
                        var Export = new DownloadListMaintPrevToExcel(pg);
                        Export.GetListExcel(ref bytes,ref filename);
                    }
                    Response.Headers.Add("x-download", filename);
                    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null){return Problem(ex.Message);}
                    return Problem(ex.InnerException.Message);
                }
            }
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            return BadRequest(errorMessages);
        }

        [AllowAnonymous]
        [HttpPost("upload-excel")]
        //[ServiceFilter(typeof(ValidationforExcell))]
        public async Task<ActionResult<Result<List<UploadExcelMaintPrevDto>>>> UploadExcel(IFormFile file)
        {
            var upload = new UploadExcelMaintPrevCommand(file);
            return await _mediator.Send(upload);
        }
    }
}
