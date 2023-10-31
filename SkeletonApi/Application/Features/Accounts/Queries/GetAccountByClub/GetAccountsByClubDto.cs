using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Accounts.Queries.GetAccountByClub
{
    public class GetAccountsByClubDto : IMapFrom<Account>
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
