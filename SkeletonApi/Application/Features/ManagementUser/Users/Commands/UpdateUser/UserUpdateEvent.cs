using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.ManagementUser.Users.Commands.UpdateUser
{
    public class UserUpdateEvent : BaseEvent
    {
        public User User { get; set; }
        public UserUpdateEvent(User user)
        {
            User = user;
        }
    }
}
