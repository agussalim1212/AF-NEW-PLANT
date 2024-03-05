using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Accounts.Profiles.Commands.DeleteAccount
{
    public record DeleteAccountRequest : IRequest<Result<Guid>>, IMapFrom<Account>
    {
        public Guid Id { get; set;}
        public DeleteAccountRequest(Guid id)
        {
            Id = id;
        }
        public DeleteAccountRequest()
        {
            
        }
    }
}
