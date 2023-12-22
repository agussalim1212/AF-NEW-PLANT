using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.Accounts.Profiles.Commands.UpdateAccount
{
    public record UpdateAccountCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int NoNRP { get; set; }
        public string PhotoURL { get; set; }
        public DateTime? BirthDate { get; set; }
    }

    internal class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateAccountCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
        {
            //var account = await _unitOfWork.Repository<Account>().GetByIdAsync(command.Id);
            //if (account != null)
            //{
            //    account.Name = command.Name;
            //    account.NoNRP = command.NoNRP;
            //    account.PhotoURL = command.PhotoURL;
            //    account.BirthDate = command.BirthDate;

            //    await _unitOfWork.Repository<Account>().UpdateAsync(account);
            //    account.AddDomainEvent(new AccountUpdatedEvent(account));

            //    await _unitOfWork.Save(cancellationToken);

            //    return await Result<Guid>.SuccessAsync(account.Id, "Account Updated.");
            //}
            //else
            //{
            return await Result<Guid>.FailureAsync("Account Not Found.");
            // }
        }
    }
}
