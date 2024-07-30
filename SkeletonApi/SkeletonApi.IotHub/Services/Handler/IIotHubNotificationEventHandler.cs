using SkeletonApi.IotHub.Model;

namespace SkeletonApi.IotHub.Services.Handler
{
    public interface IIotHubNotificationEventHandler
    {
        public IObservable<IEnumerable<NotificationModel>> Observe();
    }
}