using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Accounts.Commands.DeleteAccount
{
    public class AccountDeletedEvent : BaseEvent
    {
        public Account Account { get; }

        public AccountDeletedEvent(Account player)
        {
            Account = player;
        }
    }
}
