using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Accounts.Commands.UpdateAccount
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
