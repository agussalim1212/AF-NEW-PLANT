using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Accounts.Profiles.Commands.UpdateAccount
{
    public class AccountUpdatedEvent : BaseEvent
    {
        public Account Account { get; }

        public AccountUpdatedEvent(Account account)
        {
            Account = account;
        }
    }
}
