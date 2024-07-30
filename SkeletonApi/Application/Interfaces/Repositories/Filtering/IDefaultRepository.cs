using SkeletonApi.Application.DTOs.AirAndElectricConsumption;
using SkeletonApi.Application.DTOs.CurrentAndVoltageConsumption;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumption.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelFrontWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.TotalProduction;

namespace SkeletonApi.Application.Interfaces.Repositories.Filtering
{
    public interface IDefaultRepository
    {
        Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionDefault(string vid, string machineName, string subjectName);

        Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionDefault(string view, string vid, string machineName, string subjectName);

        Task<GetAllDetailMachineCurrentAndVoltageConsumptionDto> GetAllDetailMachineCurrentAndVoltageConsumptionDefault(string view, string vid, string machineName, string subjectName);

        //Power Consumption Summary
        Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary();

        //LIST QUALITY ASSY WHEEL REAR
        Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearFinalInspectionDefault(string vidStatus, string vidHorizontal, string vidVertikal, string vidDiskBrake);

        Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearTireInflationDefault(string vid);

        //LIST QUALITY ASSY WHEEL FRONT
        Task<List<GetListWheelFrontDto>> GetListQualityAssyWheelFrontTireInflationDefault(string vid);

        Task<List<GetListWheelFrontDto>> GetListQualityAssyWheelFrontDiskBrake(string vidTorsi);

        Task<GetAllTotalProductionDto> GetAllTotalProductionDefault(Guid machineId, string vidOK, string vidNG, string machineName);
    }
}