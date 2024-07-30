using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UploadExcel
{
    public class UploadExcelMaintPrevEvent : BaseEvent
    {
        public MaintenacePreventive maintenacePreventive { get; set; }

        public UploadExcelMaintPrevEvent(MaintenacePreventive maintenace)
        {
            maintenacePreventive = maintenace;
        }
    }
}