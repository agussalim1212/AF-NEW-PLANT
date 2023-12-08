using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
