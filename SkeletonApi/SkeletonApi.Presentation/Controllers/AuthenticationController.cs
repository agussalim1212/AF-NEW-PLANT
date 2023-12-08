using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.ManagementUser.Users.Commands.CreateUser;
using SkeletonApi.Application.Features.Users;
using SkeletonApi.Application.Features.Users.Login.Commands;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Presentation.Controllers
{

    [Route("api/authentication")]
    public class AuthenticationController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        private readonly IAuthenticationUserRepository _userRepository;
        public AuthenticationController(IAuthenticationUserRepository authenticationUser, IMediator mediator, ILogger<AuthenticationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
            _userRepository = authenticationUser;
        }


        [HttpPost]
        public async Task<ActionResult<Result<TokenDto>>> CreateUser(UserLoginRequest command)
        {
            return await _mediator.Send(command);
        }
    }
}
