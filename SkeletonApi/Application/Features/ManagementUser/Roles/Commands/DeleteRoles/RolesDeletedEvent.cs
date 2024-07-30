using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.ManagementUser.Roles.Commands.DeleteRoles
{
    public class RolesDeletedEvent : BaseEvent
    {
        public Role Role { get; set; }

        public RolesDeletedEvent(Role role)
        {
            Role = role;
        }
    }
}