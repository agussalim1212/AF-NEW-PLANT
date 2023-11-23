using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Queries.GetRoleWithPagination
{
    public class GetPermissionsWithPaginationDto : IMapFrom<GetPermissionsWithPaginationDto>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("role")]
        public string RoleName { get; set; }
        [JsonPropertyName("permissions")]
        public string Permissions { get; set; }
        [JsonPropertyName("last_created")]
        public DateTime UpdateAt { get; set; }

    }
}
