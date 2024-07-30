using SkeletonApi.Application.DTOs.AirAndElectricConsumption;
using SkeletonApi.Application.DTOs.CurrentAndVoltageConsumption;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumption.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.TotalProduction;

namespace SkeletonApi.Application.Interfaces.Repositories.Filtering
{
    public interface IYearRepository
    {
        Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionYear(string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);

        Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionYear(string view, string vid, string machineName, string subjectName, DateTime startTime, DateTime endTime);

        Task<GetAllDetailMachineCurrentAndVoltageConsumptionDto> GetAllDetailMachineCurrentAndVoltageConsumptionYear(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);

        Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary(DateTime? start, DateTime? end);

        Task<GetAllTotalProductionDto> GetAllTotalProductionYear(Guid machineId, string vidOK, string vidNG, string machineName, DateTime? start, DateTime? end);
    }
}