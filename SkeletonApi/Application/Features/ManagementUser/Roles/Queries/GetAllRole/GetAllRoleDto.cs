using SkeletonApi.Application.Common.Mappings;
using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.ManagementUser.Roles.Queries.GetAllRole
{
    public class GetAllRoleDto : IMapFrom<GetAllRoleDto>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("role")]
        public string Name { get; set; }
    }
}
