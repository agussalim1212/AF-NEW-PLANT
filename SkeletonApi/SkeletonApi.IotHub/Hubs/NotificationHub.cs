using Microsoft.AspNetCore.SignalR;
using RxSignalrStreams.Extensions;
using SkeletonApi.IotHub.Model;
using SkeletonApi.IotHub.Services.Handler;
using System.Threading.Channels;

namespace SkeletonApi.IotHub.Hubs
{
    public class NotificationHub : Hub<INotificationHub>
    {
        private readonly IotHubNotificationEventHandler _notificationEventHandler;

        public NotificationHub(IotHubNotificationEventHandler notificationHandler)
        {
            _notificationEventHandler = notificationHandler;
        }

        public ChannelReader<IEnumerable<NotificationModel>> RealtimeNotification()
        {
            return _notificationEventHandler.Observe().ToNewestValueStream(Context.ConnectionAborted);
           
        }
    }
}
