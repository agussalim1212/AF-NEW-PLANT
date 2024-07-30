using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Update
{
    public class UpdateMaintPreventiveEvent : BaseEvent
    {
        public MaintenacePreventive _updateMaintPreventive { get; set; }

        public UpdateMaintPreventiveEvent(MaintenacePreventive maintenacePreventive)
        {
            _updateMaintPreventive = maintenacePreventive;
        }
    }
}