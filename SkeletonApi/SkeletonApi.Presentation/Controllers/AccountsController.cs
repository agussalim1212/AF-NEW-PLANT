using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.Accounts;
using SkeletonApi.Application.Features.Accounts.Password.Command.ChangesPassword;
using SkeletonApi.Application.Features.Accounts.Profiles.Commands.CreateAccount;
using SkeletonApi.Application.Features.Accounts.Profiles.Commands.DeleteAccount;
using SkeletonApi.Application.Features.Accounts.Profiles.Queries.GetAllAccountsByUsername;
using SkeletonApi.Shared;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("api/account")]
    public class AccountsController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        //[HttpGet("trigger")]
        //public async Task<IActionResult> GetTrigger()
        //{
        //    using (var scope = _serviceScopeFactory.CreateScope())
        //    {
        //        var scoped = scope.ServiceProvider.GetRequiredService<IClientRequest>();
        //        await scoped.GetTask("DCM/P9AUA0/testing", "test");
        //        return Ok("Message published successfully");
        //    }
        //}

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

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result<Guid>>> DeleteUser(Guid id)
        {
            return await _mediator.Send(new DeleteAccountRequest(id));
        }
    }
}