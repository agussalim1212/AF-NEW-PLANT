using SkeletonApi.Application.DTOs.AirAndElectricConsumption;
using SkeletonApi.Application.DTOs.CurrentAndVoltageConsumption;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumption.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.TotalProduction;

namespace SkeletonApi.Application.Interfaces.Repositories.Filtering
{
    public interface IMonthRepository
    {
        Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionMonth(string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);

        Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionMonth(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);

        Task<GetAllDetailMachineCurrentAndVoltageConsumptionDto> GetAllDetailMachineCurrentAndVoltageConsumptionMonth(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);

        Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary(DateTime? start, DateTime? end);

        Task<GetAllTotalProductionDto> GetAllTotalProductionMonth(Guid machineId, string vidOK, string vidNG, string machineName, DateTime? start, DateTime? end);
    }
}