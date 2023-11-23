using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
