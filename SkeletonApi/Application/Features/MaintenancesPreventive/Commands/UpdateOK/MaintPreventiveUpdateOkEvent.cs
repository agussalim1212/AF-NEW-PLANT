using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UpdateOK
{
    public class MaintPreventiveUpdateOkEvent : BaseEvent
    {
        public MaintenacePreventive _maintenacePreventive { get; }

        public MaintPreventiveUpdateOkEvent(MaintenacePreventive maintPreventive)
        {
            _maintenacePreventive = maintPreventive;
        }
    }
}