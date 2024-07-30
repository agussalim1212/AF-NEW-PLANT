using MediatR;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Commands.CreatePermissions
{
    public record CreatePermissionsRequest : IRequest<Result<CreatePermissionsResponseDto>>
    {
        [JsonPropertyName("role_name")]
        public string RoleName { get; set; }
        [JsonPropertyName("claim")]
        public List<ClaimType> Claim { get; set; }
    }
}