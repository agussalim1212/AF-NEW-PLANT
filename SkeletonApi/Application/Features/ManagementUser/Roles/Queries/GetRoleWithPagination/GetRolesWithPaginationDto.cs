using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

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