using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Accounts.Profiles.Commands.DeleteAccount
{
    public record DeleteAccountRequest : IRequest<Result<Guid>>, IMapFrom<Account>
    {
        public Guid Id { get; set; }
        public DeleteAccountRequest(Guid id)
        {
            Id = id;
        }
        public DeleteAccountRequest()
        {
        }
    }
}