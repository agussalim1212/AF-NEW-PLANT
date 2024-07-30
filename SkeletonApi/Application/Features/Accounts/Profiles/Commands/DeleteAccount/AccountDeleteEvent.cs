using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Accounts.Profiles.Commands.DeleteAccount
{
    public class AccountDeleteEvent : BaseEvent
    {
        public Account Account { get; set; }

        public AccountDeleteEvent(Account account)
        {
            Account = account;
        }
    }
}