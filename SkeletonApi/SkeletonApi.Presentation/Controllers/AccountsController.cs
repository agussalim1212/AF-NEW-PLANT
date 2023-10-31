using SkeletonApi.Application.Features.Accounts.Commands.CreateAccount;
using SkeletonApi.Application.Features.Accounts.Commands.DeleteAccount;
using SkeletonApi.Application.Features.Accounts.Commands.UpdateAccount;
using SkeletonApi.Application.Features.Accounts.Queries.GetAccountsWithPagination;
using SkeletonApi.Application.Features.Accounts.Queries.GetAllAccounts;
using SkeletonApi.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkeletonApi.Application.Features.Accounts.Queries.GetAccountByClub;
using SkeletonApi.Application.Features.Accounts.Queries.GetAccountWithPagination;
using SkeletonApi.Application.Features.Accounts.Queries.GetAccountById;
using Microsoft.Extensions.Logging;

namespace SkeletonApi.Presentation.Controllers
{
    public class AccountsController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Result<List<GetAllAccountsDto>>>> Get()
        {
            _logger.LogInformation("Here is info message from our values controller.");
            _logger.LogDebug("Here is debug message from our values controller.");
            _logger.LogWarning("Here is warn message from our values controller.");
            _logger.LogError("Here is an error message from our values controller.");
            //throw new Exception();
            //return Ok("OK");
            return await _mediator.Send(new GetAllAccountsQuery());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<GetAccountByIdDto>>> GetAccountsById(Guid id)
        {
            return await _mediator.Send(new GetAccountByIdQuery(id));
        }

        [HttpGet]
        [Route("club/{clubId}")]
        public async Task<ActionResult<Result<List<GetAccountsByClubDto>>>> GetAccountsByClub(Guid clubId)
        {
            return await _mediator.Send(new GetAccountsByClubQuery(clubId));
        }

        [HttpGet]
        [Route("paged")]
        public async Task<ActionResult<PaginatedResult<GetAccountsWithPaginationDto>>> GetAccountsWithPagination([FromQuery] GetAccountsWithPaginationQuery query)
        {
            var validator = new GetAccountsWithPaginationValidator();

            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var result = validator.Validate(query);

            if (result.IsValid)
            {
                return await _mediator.Send(query);
            }

            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            return BadRequest(errorMessages);
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create(CreateAccountCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<Guid>>> Update(Guid id, UpdateAccountCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            return await _mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<Guid>>> Delete(Guid id)
        {
            return await _mediator.Send(new DeleteAccountCommand(id));
        }
    }
}