using SkeletonApi.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.ManagementUser.Roles.Queries.GetRoleWithPagination
{
    public class GetRolesWithPaginationDto : IMapFrom<GetRolesWithPaginationDto>
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("role")]
        public string? Name { get; set; }
        [JsonPropertyName("last_created")]
        public DateTime? UpdateAt { get; set; }
    }
}
