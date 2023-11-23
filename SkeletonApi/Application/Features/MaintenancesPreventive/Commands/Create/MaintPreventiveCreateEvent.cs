using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Create
{
    internal class MaintPreventiveCreateEvent : BaseEvent
    {
        public MaintenacePreventive maintenacePreventive { get; set; }

        public MaintPreventiveCreateEvent(MaintenacePreventive maintPreventive)
        {
                maintenacePreventive = maintPreventive;
        }
    }
}
