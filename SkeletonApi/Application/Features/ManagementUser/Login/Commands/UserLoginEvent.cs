using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Users.Login.Commands
{
    public class UserLoginEvent : BaseEvent
    {
        public User User { get; set; }

        public UserLoginEvent(User user)
        {
            User = user;
        }
    }
}
