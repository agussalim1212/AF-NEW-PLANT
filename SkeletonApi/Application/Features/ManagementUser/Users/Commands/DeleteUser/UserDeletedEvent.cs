using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.ManagementUser.Users.Commands.DeleteUser
{
    public class UserDeletedEvent : BaseEvent
    {
        public User User { get; set; }

        public UserDeletedEvent(User user)
        {
            User = user;
        }
    }
}