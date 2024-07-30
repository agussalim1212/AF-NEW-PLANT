using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Machines.Commands.UpdateMachines
{
    public class MachinesUpdateEvent : BaseEvent
    {
        public Machine Machines { get; set; }

        public MachinesUpdateEvent(Machine machine)
        {
            Machines = machine;
        }
    }
}