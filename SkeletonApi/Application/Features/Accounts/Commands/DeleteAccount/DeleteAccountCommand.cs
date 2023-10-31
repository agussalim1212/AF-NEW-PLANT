using AutoMapper;
using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Accounts.Commands.DeleteAccount
{
    public record DeleteAccountCommand : IRequest<Result<Guid>>, IMapFrom<Account>
    {
        public Guid Id { get; set; }

        public DeleteAccountCommand()
        {

        }

        public DeleteAccountCommand(Guid id)
        {
            Id = id;
        }
    }

    internal class DeletePlayerCommandHandler : IRequestHandler<DeleteAccountCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeletePlayerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(command.Id);
            if (account != null)
            {
                await _unitOfWork.Repository<Account>().DeleteAsync(account);
                account.AddDomainEvent(new AccountDeletedEvent(account));

                await _unitOfWork.Save(cancellationToken);

                return await Result<Guid>.SuccessAsync(account.Id, "Product Deleted");
            }
            else
            {
                return await Result<Guid>.FailureAsync("Player Not Found.");
            }
        }
    }
}
