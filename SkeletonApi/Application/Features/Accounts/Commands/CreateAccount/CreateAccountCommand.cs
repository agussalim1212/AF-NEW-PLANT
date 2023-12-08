using MediatR;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using SkeletonApi.Application.Common.Mappings;
using AutoMapper;
using SkeletonApi.Application.Interfaces.Repositories;

namespace SkeletonApi.Application.Features.Accounts.Commands.CreateAccount
{
    public record CreateAccountCommand : IRequest<Result<Guid>>, IMapFrom<Account>
    {
        public string Name { get; set; }
        public int NoNRP { get; set; }
        public string PhotoURL { get; set; }
        public DateTime BirthDate { get; set; }
    }

    internal class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateAccountCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
        {
            var account = new Account()
            {
                Name = command.Name,
                NoNRP = command.NoNRP,
                PhotoURL = command.PhotoURL,
                BirthDate = command.BirthDate
            };

            await _unitOfWork.Repository<Account>().AddAsync(account);
            account.AddDomainEvent(new AccountCreatedEvent(account));
            await _unitOfWork.Save(cancellationToken);
            return await Result<Guid>.SuccessAsync(account.Id, "Account Created.");
        }
    }
}
