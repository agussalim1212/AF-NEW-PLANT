using MediatR;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ManagementUser.Roles.Commands.UpdateRoles
{
    public class UpdateRolesRequest : IRequest<Result<Role>>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("role")]
        public string Name { get; set; }
    }
}