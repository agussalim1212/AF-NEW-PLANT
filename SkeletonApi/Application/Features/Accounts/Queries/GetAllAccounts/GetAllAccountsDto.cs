using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Accounts.Queries.GetAllAccounts
{
    public class GetAllAccountsDto : IMapFrom<Account>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public int NoNRP { get; init; }
        public string FacebookUrl { get; init; }
        public string TwitterUrl { get; init; }
        public string InstagramUrl { get; init; }
        public int DisplayOrder { get; init; }
    }
}
