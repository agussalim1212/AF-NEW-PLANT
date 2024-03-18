using SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.TotalProduction;


namespace SkeletonApi.Application.Interfaces
{
    public interface IDetailGensubRespository
    {
        Task<GetAllTotalProductionGensubDto> GetAllTotalProductionGensubDto(Guid machineId, string type, DateTime start, DateTime end);
       
    }
}
