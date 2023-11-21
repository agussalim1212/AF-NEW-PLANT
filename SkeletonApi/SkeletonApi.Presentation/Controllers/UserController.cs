using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Shared;
using SkeletonApi.Application.Features.Users;
using SkeletonApi.Application.Features.Users.CreateUser;

namespace SkeletonApi.Presentation.Controllers
{
    public class UserController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("create-user")]
        public async Task<ActionResult<Result<CreateUserResponseDto>>> CreateUser(CreateUserRequest command)
        {
            return await _mediator.Send(command);
        }


    }
}
