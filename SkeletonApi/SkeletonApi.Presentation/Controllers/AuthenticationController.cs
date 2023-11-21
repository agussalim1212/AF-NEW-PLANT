using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.Users;
using SkeletonApi.Application.Interfaces.Repositories;

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
       // [ServiceFilter(typeof(ValidationFilterAttribute))]
       
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            var result = await _userRepository.RegisterUser(userForRegistration);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return StatusCode(201);
        }

        [HttpPost("login")]
       // [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            if (!await _userRepository.ValidateUser(user))
                return Unauthorized();
            var tokenDto = await _userRepository.CreateToken(populateExp: true);
            return Ok(tokenDto);
        }
    }
}
