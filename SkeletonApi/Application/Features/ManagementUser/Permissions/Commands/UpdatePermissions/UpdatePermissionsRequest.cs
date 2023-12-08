using MediatR;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Commands.UpdatePermissions
{
    public class UpdatePermissionsRequest : IRequest<Result<Permission>>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("role_name")]
        public string RoleName { get; set; }
        [JsonPropertyName("claim")]
        public List<ClaimType> Claim { get; set; }
    }
}
