using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Accounts.Profiles.Commands.DeleteAccount
{
    public class AccountDeletedEvent : BaseEvent
    {
        public Account Account { get; }

        public AccountDeletedEvent(Account account)
        {
            Account = account;
        }
    }
}
