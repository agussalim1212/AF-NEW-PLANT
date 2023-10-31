using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
