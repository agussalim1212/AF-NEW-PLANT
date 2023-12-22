using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UploadExcel
{
    public class UploadExcelMaintPrevEvent : BaseEvent
    {
        public MaintenacePreventive maintenacePreventive {  get; set; }

        public UploadExcelMaintPrevEvent(MaintenacePreventive maintenace)
        {
            maintenacePreventive = maintenace;
        }
    }
}
