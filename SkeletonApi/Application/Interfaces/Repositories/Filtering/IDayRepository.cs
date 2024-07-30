using SkeletonApi.Application.DTOs.AirAndElectricConsumption;
using SkeletonApi.Application.DTOs.CurrentAndVoltageConsumption;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumption.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelFrontWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.TotalProduction;

namespace SkeletonApi.Application.Interfaces.Repositories.Filtering
{
    public interface IDayRepository
    {
        Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionAsync(string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);

        Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionAsync(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);
        Task<GetAllDetailMachineCurrentAndVoltageConsumptionDto> GetAllDetailMachineCurrentAndVoltageConsumptionDay(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime);

        //START LIST QUALITY ASSY WHEEL REAR
        Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearFinalInspectioDay(string vidStatus, string vidHorizontal, string vidVertikal, string vidDiskBrake, Guid machineId, DateTime? Start, DateTime? End);

        Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearTireInflationDay(string vid, string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End);

        //END LIST QUALITY ASSY WHEEL REAR
        Task<List<GetListWheelFrontDto>> GetListQualityWheelFrontFinalInspectionDay(string vidStatus, string vidHorizontal, string vidVertikal, string vidDiskBrake, Guid machineId, DateTime? Start, DateTime? End);

        Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary(DateTime? start, DateTime? end);

        Task<GetAllTotalProductionDto> GetAllTotalProductionDay(Guid machineId, string vidOK, string vidNG, string machineName, DateTime? start, DateTime? end);
    }
}