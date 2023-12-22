using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Application.Features.Accounts.Password.Command.ChangesPassword
{
    public class PasswordUpdatedEvent : BaseEvent
    {
        public User User { get; set; }
        public PasswordUpdatedEvent(User user)
        {
            User = user;
        }
    }
}
