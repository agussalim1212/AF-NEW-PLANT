using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Commands.CreatePermissions
{
    public class PermissionnsCreatedEvent : BaseEvent
    {
        public Permission Permissions { get; set; }

        public PermissionnsCreatedEvent(Permission permission)
        {
            Permissions = permission;
        }
    }
}