using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.ManagementUser.Users.Commands.CreateUser
{
    public class UserCreatedEvent : BaseEvent
    {
        public User User { get; set; }

        public UserCreatedEvent(User user)
        {
            User = user;
        }
    }
}