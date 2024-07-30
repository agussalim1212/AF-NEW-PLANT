using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.ManagementUser.Roles.Commands.CreateRoles
{
    public class RolesCreatedEvent : BaseEvent
    {
        public Role Role { get; set; }

        public RolesCreatedEvent(Role role)
        {
            Role = role;
        }
    }
}