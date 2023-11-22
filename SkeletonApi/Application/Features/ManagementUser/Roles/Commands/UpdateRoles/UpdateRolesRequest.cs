using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
