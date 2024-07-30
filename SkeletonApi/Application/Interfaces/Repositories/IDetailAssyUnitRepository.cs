using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityAssyUnitLineWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityRobotScanImage;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFilingWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityMainLineWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityOilBrakeWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityPressConeRaceWithPagination;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDetailAssyUnitRepository
    {
        Task<List<GetListQualityCoolantFilingDto>> GetAllListQualityCoolantFiling(Guid machineId, string type, DateTime start, DateTime end);

        Task<List<GetListQualityMainLineDto>> GetAllListQualityMainLine(Guid machineId, string type, DateTime start, DateTime end);

        Task<List<GetListQualityNutRunnerSteeringStemDto>> GetAllListQualityNutRunnerStem(Guid machineId, string type, DateTime start, DateTime end);

        Task<List<GetListQualityOilBrakeDto>> GetAllListQualityOilBrake(Guid machineId, string type, DateTime start, DateTime end);

        Task<List<GetListQualityPressConeRaceDto>> GetAllListQualityPressConeRace(Guid machineId, string type, DateTime start, DateTime end);

        Task<List<GetListQualityRobotScanImageDto>> GetAllListQualityRobotScanImage(Guid machineId, string type, DateTime start, DateTime end);
    }
}