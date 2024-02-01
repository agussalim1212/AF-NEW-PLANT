using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.AirConsumption;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ElectricGeneratorConsumption;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.EnergyConsumptionAssyUnitLine;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.FrequencyInverter;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityAssyUnitLineWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFiling;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityMainLine;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityOilBrake;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityPressConeRace;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityRobotScanImage;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.MachineInformation;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.TotalProduction;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDetailAssyUnitRepository
    {
      
        Task<GetAllMachineInformationDto> GetAllMachineInformationAsync(Guid machine_id);
        Task<GetAllTotalProductionDto> GetAllTotalProduction(Guid machine_id, string type, DateTime start, DateTime end);
        Task<GetAllFrequencyInverterDto> GetAllFrequencyInverter(Guid machine_id, string type, DateTime start, DateTime end);
        Task<GetAllAirConsumptionDto> GetAllAirConsumption(Guid machine_id, string type, DateTime start, DateTime end);
        Task<GetAllElectricGeneratorConsumptionDto> GetAllElectricGeneratorConsumption(Guid machine_id, string type, DateTime start,DateTime end);
        Task<GetAllEnergyConsumptionDto> GetAllEnergyConsumption(Guid machine_id, string type,DateTime start,DateTime end);
        Task<List<GetListQualityCoolantFilingDto>> GetAllListQualityCoolantFiling(Guid machineId, string type, DateTime start, DateTime end);
        Task<List<GetListQualityMainLineDto>> GetAllListQualityMainLine(Guid machineId, string type, DateTime start, DateTime end);
        Task<List<GetListQualityNutRunnerSteeringStemDto>> GetAllListQualityNutRunnerStem(Guid machineId, string type, DateTime start, DateTime end);
        Task<List<GetListQualityOilBrakeDto>> GetAllListQualityOilBrake(Guid machineId, string type, DateTime start, DateTime end);
        Task<List<GetListQualityPressConeRaceDto>> GetAllListQualityPressConeRace(Guid machineId, string type, DateTime start, DateTime end);
        Task<List<GetListQualityRobotScanImageDto>> GetAllListQualityRobotScanImage(Guid machineId, string type, DateTime start, DateTime end);
        //Energy Consumption Summary yang Di Machine Information
        Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary(string type, DateTime start, DateTime end);
    }
}
