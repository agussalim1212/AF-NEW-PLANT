using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Accounts.Profiles.Queries.GetAllAccountsByUsername
{
    public class GetAllAccountsDto : IMapFrom<GetAllAccountsDto>
    {
        public Guid Id { get; init; }
        public string Email { get; init; }
        public string Username { get; init; }
        public string Foto { get; init; }

    }
}
