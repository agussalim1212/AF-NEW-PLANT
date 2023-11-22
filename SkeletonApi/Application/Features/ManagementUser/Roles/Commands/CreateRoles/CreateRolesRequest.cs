using MediatR;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.ManagementUser.Roles.Commands.CreateRoles
{
    public record CreateRolesRequest : IRequest<Result<CreateRolesResponseDto>>
    {
        [Required(ErrorMessage = "Role Name is required")]
        [JsonPropertyName("role")]
        public string? Name { get; init; }
    }
}
