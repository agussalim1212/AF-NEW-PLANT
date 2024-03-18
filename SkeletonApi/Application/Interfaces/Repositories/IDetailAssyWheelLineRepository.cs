using SkeletonApi.Application.DTOs.DetailMachine;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDetailAssyWheelLineRepository
    {
        Task<GetVidSubjectDto> GetVidsAsync(Guid machineId, string vid);
  


    
    }
}
