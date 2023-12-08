using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
