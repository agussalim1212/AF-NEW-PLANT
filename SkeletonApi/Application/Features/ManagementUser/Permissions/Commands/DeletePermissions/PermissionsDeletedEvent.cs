using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Commands.DeletePermissions
{
    public class PermissionsDeletedEvent : BaseEvent
    {
        public Permission Permission { get; set; }

        public PermissionsDeletedEvent(Permission permission)
        {
            Permission = permission;
        }
    }
}