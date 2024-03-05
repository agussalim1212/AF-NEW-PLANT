using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
