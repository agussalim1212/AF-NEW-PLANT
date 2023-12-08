using SkeletonApi.IotHub.Model;

namespace SkeletonApi.IotHub.Services.Handler
{
    public interface IIotHubMachineHealthEventHandler
    {
        public IObservable<IEnumerable<MachineHealthModel>> Observe();
    }
}
