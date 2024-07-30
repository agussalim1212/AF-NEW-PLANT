using SkeletonApi.Infrastructure.DTOs;
using System.Threading.Channels;

namespace SkeletonApi.Infrastructure.Hubs
{
    public interface IDataHub
    {
        public ChannelReader<IEnumerable<MachineDto>> RealtimeMachine();
    }
}