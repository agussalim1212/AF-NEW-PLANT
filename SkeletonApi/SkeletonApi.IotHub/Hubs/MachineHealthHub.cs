using Microsoft.AspNetCore.SignalR;
using RxSignalrStreams.Extensions;
using SkeletonApi.IotHub.Model;
using SkeletonApi.IotHub.Services.Handler;
using System.Threading.Channels;

namespace SkeletonApi.IotHub.Hubs
{
    public class MachineHealthHub : Hub<IMachineHealthHub>
    {
        private readonly IotHubMachineHealthEventHandler _machineHealthEventHandler;

        public MachineHealthHub(IotHubMachineHealthEventHandler machineHealthEventHandler)
        {
            _machineHealthEventHandler = machineHealthEventHandler;
        }

        public ChannelReader<IEnumerable<MachineHealthModel>> RealtimeMachine()
        {
            return _machineHealthEventHandler.Observe().ToNewestValueStream(Context.ConnectionAborted);
        }
    }
}