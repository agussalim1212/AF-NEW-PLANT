using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Notification.Commands.Update
{
   public class UpdateNotifEvent : BaseEvent
    {
        public Notifications _notifications { get; set; }

        public UpdateNotifEvent(Notifications notifications)
        {
           _notifications = notifications;
        }
    }
}
