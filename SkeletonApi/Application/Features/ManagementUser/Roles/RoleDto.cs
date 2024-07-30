using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ManagementUser.Roles
{
    public record RoleDto
    {
        [Required(ErrorMessage = "Role Name is required")]
        [JsonPropertyName("name")]
        public string? Name { get; init; }
    }
    public sealed record CreateRolesResponseDto : RoleDto { }
}