using MediatR;
using SkeletonApi.Shared;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ManagementUser.Roles.Commands.CreateRoles
{
    public record CreateRolesRequest : IRequest<Result<CreateRolesResponseDto>>
    {
        [Required(ErrorMessage = "Role Name is required")]
        [JsonPropertyName("role")]
        public string? Name { get; init; }
    }
}