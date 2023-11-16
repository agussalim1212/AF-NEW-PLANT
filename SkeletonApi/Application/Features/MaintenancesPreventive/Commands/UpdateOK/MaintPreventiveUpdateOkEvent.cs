using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UpdateOK
{
    public class MaintPreventiveUpdateOkEvent : BaseEvent
    {
        public MaintenacePreventive _maintenacePreventive { get;}

        public MaintPreventiveUpdateOkEvent(MaintenacePreventive maintPreventive)
        {
            _maintenacePreventive = maintPreventive;
        }
    }
}
