using SkeletonApi.IotHub.Model;
using System.Threading.Channels;

namespace SkeletonApi.IotHub.Hubs
{
    public interface INotificationHub
    {
        public ChannelReader<IEnumerable<MachineHealthModel>> RealtimeNotification();
    }
}