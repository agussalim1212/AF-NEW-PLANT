using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
