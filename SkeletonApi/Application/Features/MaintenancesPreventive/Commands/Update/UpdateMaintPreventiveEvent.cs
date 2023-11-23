using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
