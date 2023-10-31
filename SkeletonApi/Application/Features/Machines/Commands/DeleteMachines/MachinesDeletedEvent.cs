using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
