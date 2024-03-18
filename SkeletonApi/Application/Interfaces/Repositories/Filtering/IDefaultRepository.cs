using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AmpereConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories.Filtering
{
    public interface IDefaultRepository
    {
        Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionDefault(string vid, string machineName, string subjectName);
        Task<GetAllDetailMachineAirConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionDefault(string view, string vid, string machineName, string subjectName);
        Task<GetAllDetailMachineCurrentAndVoltageConsumptionDto> GetAllDetailMachineCurrentAndVoltageConsumptionDefault(string view, string vid, string machineName, string subjectName);

        //Power Consumption Summary
        Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary();

        //LIST QUALITY ASSY WHEEL REAR
        Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearFinalInspectionDefault(string vidStatus, string vidHorizontal, string vidVertikal, string vidDiskBrake, Guid machineId, DateTime? Start, DateTime? End);

        Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearTireInflationDefault(string vid, string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End);
    }
}
