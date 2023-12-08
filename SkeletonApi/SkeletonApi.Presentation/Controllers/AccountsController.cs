using SkeletonApi.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.Accounts;
using SkeletonApi.Application.Features.Accounts.Profiles.Queries.GetAllAccountsByUsername;
using SkeletonApi.Application.Features.Accounts.Profiles.Commands.CreateAccount;
using SkeletonApi.Application.Features.Accounts.Password.Command.ChangesPassword;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("api/account")]
    public class AccountsController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<Result<List<GetAllAccountsDto>>>> GetAccount(string username)
        {
            //_logger.LogInformation("Here is info message from our values controller.");
            //_logger.LogDebug("Here is debug message from our values controller.");
            //_logger.LogWarning("Here is warn message from our values controller.");
            //_logger.LogError("Here is an error message from our values controller.");
            
            return await _mediator.Send(new GetAllAccountsQuery(username));
        }


        [HttpPost("{username}")]
        public async Task<ActionResult<Result<CreateAccountResponseDto>>> Create(string username, [FromForm] CreateAccountRequest command)
        {
            command.Username = username;
            return await _mediator.Send(command);
        }

        [HttpPut("{username}")]
        public async Task<ActionResult<Result<string>>> Update(string username, UpdatePasswordCommand command)
        {
            if (username != command.Username)
            {
                return BadRequest();
            }
            return await _mediator.Send(command);
        }

    }
}