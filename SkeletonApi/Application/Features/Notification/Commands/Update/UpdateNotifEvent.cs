using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

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