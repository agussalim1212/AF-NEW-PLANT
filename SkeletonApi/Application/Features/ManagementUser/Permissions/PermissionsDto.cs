using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ManagementUser.Permissions
{
    public record PermissionsDto
    {
        [JsonPropertyName("role_name")]
        public string RoleName{ get; set; }
        [JsonPropertyName("claim")]
        public List<ClaimType> Claim { get; set; }
    }

    public record ClaimType
    {
        [JsonPropertyName("claim_value")]
        public string ClaimValue { get; set; }
    }
 
    public sealed record CreatePermissionsResponseDto : PermissionsDto { }
}
