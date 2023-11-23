using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.ManagementUser.Roles;
using SkeletonApi.Application.Features.ManagementUser.Roles.Commands.CreateRoles;
using SkeletonApi.Application.Features.ManagementUser.Roles.Commands.DeleteRoles;
using SkeletonApi.Application.Features.ManagementUser.Roles.Commands.UpdateRoles;
using SkeletonApi.Application.Features.ManagementUser.Roles.Queries.GetAllRole;
using SkeletonApi.Application.Features.ManagementUser.Roles.Queries.GetRoleWithPagination;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Presentation.ActionFilter;
using SkeletonApi.Shared;
using System.Text.Json;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("api/role")]
    public class RoleController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public RoleController(IMediator mediator, ILogger<RoleController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<CreateRolesResponseDto>>> CreateRoles(CreateRolesRequest command)
        {
            return await _mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<string>>> DeleteRoles(string id)
        {
            return await _mediator.Send(new DeleteRolesRequest(id));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<Role>>> UpdateRoles(string id, UpdateRolesRequest command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }


        [HttpGet("get-all-role")]
        [ServiceFilter(typeof(AuditLoggingFilter))]
        public async Task<ActionResult<Result<List<GetAllRoleDto>>>> GetAll()
        {
            return await _mediator.Send(new GetAllRoleQuery());
        }

        [HttpGet("list-role")]
        public async Task<ActionResult<PaginatedResult<GetRolesWithPaginationDto>>> GetUserWithPagination([FromQuery] GetRolesWithPaginationQuery query)
        {
            var validator = new GetRolesWithPaginationValidator();
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

    }

}
