using DocumentFormat.OpenXml.Wordprocessing;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;


namespace SkeletonApi.Application.Interfaces.Repositories.Filtering
{
    public interface IDayRepository
    {
        Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionAsync(string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);
        Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionAsync(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);
        Task<List<GetListWheelRearDto>> GetListQualityAssyWheelFinalInspection(string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End);
        Task<List<GetListWheelRearDto>> GetListQualityAssyWheelTireInflation(string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End);
        Task<List<GetListWheelRearDto>> GetListQualityAssyWheelDiskBrake(string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End);
        Task<List<GetListWheelRearDto>> GetListQualityAssyWheelPressBearing(string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End);

        Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary(DateTime? start, DateTime? end);


    }
}
