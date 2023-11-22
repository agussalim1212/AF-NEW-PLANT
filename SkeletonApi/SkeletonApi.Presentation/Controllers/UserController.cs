using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Shared;
using SkeletonApi.Application.Features.Users;
using SkeletonApi.Domain.Entities;
using System.Text.Json;
using SkeletonApi.Application.Features.ManagementUser.Users.Commands.CreateUser;
using SkeletonApi.Application.Features.ManagementUser.Users.Commands.DeleteUser;
using SkeletonApi.Application.Features.ManagementUser.Users.Commands.UpdateUser;
using SkeletonApi.Application.Features.ManagementUser.Users.Queries.GetUserWithPagination;


namespace SkeletonApi.Presentation.Controllers
{
    [Route("api/users")]
    public class UserController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<CreateUserResponseDto>>> CreateUser(CreateUserRequest command)
        {
            return await _mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<string>>> DeleteUser(string id)
        {
            return await _mediator.Send(new DeleteUserRequest(id));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<User>>> UpdateUser(string id, UpdateUserRequest command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }

        [HttpGet("list-users")]
        public async Task<ActionResult<PaginatedResult<GetUserWithPaginationDto>>> GetUserWithPagination([FromQuery] GetUserWithPaginationQuery query)
        {
            var validator = new GetUserWithPaginationValidator();
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
