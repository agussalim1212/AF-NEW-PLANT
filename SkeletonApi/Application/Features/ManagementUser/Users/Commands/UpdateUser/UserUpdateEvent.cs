using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

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