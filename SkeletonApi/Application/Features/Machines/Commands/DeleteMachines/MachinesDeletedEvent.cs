using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Application.Features.Machines.Commands.DeleteMachines
{
    public class MachinesDeletedEvent : BaseEvent
    {
        public Machine Machine { get; set; }
        public MachinesDeletedEvent(Machine machine)
        {
            Machine = machine;
        }
    }
}
