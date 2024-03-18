using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AmpereConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;


namespace SkeletonApi.Application.Interfaces.Repositories.Filtering
{
    public interface IWeekRepository
    {
        Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionWeek(string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);
        Task<GetAllDetailMachineAirConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionWeek(string view,string vid, string machineName, string subjectName, DateTime startTime, DateTime endTime);
        Task<GetAllDetailMachineCurrentAndVoltageConsumptionDto> GetAllDetailMachineCurrentAndVoltageConsumptionWeek(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);

        Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary(DateTime? start, DateTime? end);
    }
}
