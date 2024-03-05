using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.MachineInformationAssyWheelLine;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDetailAssyWheelLineRepository
    {
        Task<List<GetVid>> GetVidsAsync(Guid machineId, string category);
        Task<GetAllMachineInformationAssyWheelLineDto> GetAllMachineInformationAsync(Guid machine_id);
     


    
    }
}
