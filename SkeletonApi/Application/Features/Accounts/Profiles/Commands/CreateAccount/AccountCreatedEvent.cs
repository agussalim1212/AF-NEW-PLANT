using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Accounts.Profiles.Commands.CreateAccount
{
    public class AccountCreatedEvent : BaseEvent
    {
        public Account Account { get; set; }

        public AccountCreatedEvent(Account account)
        {
            Account = account;
        }
    }
}