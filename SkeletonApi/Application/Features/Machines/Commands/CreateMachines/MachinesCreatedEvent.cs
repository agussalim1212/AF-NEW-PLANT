using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
namespace SkeletonApi.Application.Features.Machines.Commands.CreateMachines
{
    public class MachinesCreatedEvent : BaseEvent
    {
        public Machine Machine { get; set; }

        public MachinesCreatedEvent(Machine machine)
        {
            Machine = machine;
        }
    }
}
