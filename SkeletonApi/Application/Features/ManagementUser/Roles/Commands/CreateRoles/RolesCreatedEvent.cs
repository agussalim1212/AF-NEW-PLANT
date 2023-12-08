using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
