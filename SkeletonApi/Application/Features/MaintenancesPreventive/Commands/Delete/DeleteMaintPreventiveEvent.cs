using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
