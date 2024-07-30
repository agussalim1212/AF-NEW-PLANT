using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Commands.UpdatePermissions
{
    public class PermissionsUpdateEvent : BaseEvent
    {
        public Permission Permission { get; set; }

        public PermissionsUpdateEvent(Permission permission)
        {
            Permission = permission;
        }
    }
}