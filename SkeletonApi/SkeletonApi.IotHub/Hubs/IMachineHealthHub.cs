using SkeletonApi.IotHub.Model;
using System.Threading.Channels;

namespace SkeletonApi.IotHub.Hubs
{
    public interface IMachineHealthHub
    {
        public ChannelReader<IEnumerable<NotificationModel>> RealtimeNotification();
    }
}