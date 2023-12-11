using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.Notification.Commands.Update;
using SkeletonApi.Application.Features.Notification.Queries.GetAllNotif;
using SkeletonApi.Application.Features.Notification.Queries.GetListNotif;
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
    [Route("/api/notif")]
    public class NotificationsController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;

        public NotificationsController(IMediator mediator, ILogger<Notifications> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("get-list-notif")]
        public async Task<ActionResult<Result<List<GetListNotifDto>>>> GetListNotif()
        {
            return await _mediator.Send(new GetListNotifQuery());
        }

        [HttpGet("get-all-notif")]
        public async Task<ActionResult<PaginatedResult<GetAllNotifDto>>> GetAllNotif([FromQuery] GetAllNotifQuery query)
        {
            var validator = new GetAllNotifValidator();

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

        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<Result<UpdateNotifDto>>> Update(Guid id, [FromBody] UpdateNotifCommand command)
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
    }
}
