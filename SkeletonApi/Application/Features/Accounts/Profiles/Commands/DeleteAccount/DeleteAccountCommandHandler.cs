using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.Accounts.Profiles.Commands.DeleteAccount
{
    internal class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountRequest, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteAccountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteAccountRequest request, CancellationToken cancellationToken)
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(request.Id);
            if (account != null)
            {
                await _unitOfWork.Repository<Account>().DeleteAsync(account);
                account.AddDomainEvent(new AccountDeleteEvent(account));
                await _unitOfWork.Save(cancellationToken);

                return await Result<Guid>.SuccessAsync(account.Id, "Account Deleted.");
            }
            return await Result<Guid>.FailureAsync("Account Not Found");
        }
    }
}
