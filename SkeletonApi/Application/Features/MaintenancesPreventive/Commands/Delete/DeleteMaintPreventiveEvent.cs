using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Delete
{
    public class DeleteMaintPreventiveEvent : BaseEvent
    {
        public MaintenacePreventive maintenacePreventive { get; }

        public DeleteMaintPreventiveEvent(MaintenacePreventive maintenace)
        {
            maintenacePreventive = maintenace;
        }
    }
}