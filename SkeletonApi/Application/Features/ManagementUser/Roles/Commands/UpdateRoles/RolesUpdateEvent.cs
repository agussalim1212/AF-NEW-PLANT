using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.ManagementUser.Roles.Commands.UpdateRoles
{
    public class RolesUpdateEvent : BaseEvent
    {
        public Role Role { get; set; }

        public RolesUpdateEvent(Role role)
        {
            Role = role;
        }
    }
}