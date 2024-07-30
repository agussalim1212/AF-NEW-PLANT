using SkeletonApi.Application.DTOs.DetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityDataBarcodeWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.MachineInformation;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDetailMachineRepository
    {
        Task<GetAllDetailMachineEnergyConsumptionDto> GetSubjectPowerAsync(Guid machineId);

        Task<GetVidSubjectDto> GetSubjectAsync(Guid machineId, string vid);

        Task<GetAllMachineInformationDto> GetAllMachineInformationAsync(Guid machine_id, string vidRunning, string vidReminder, string machineName);

        Task<List<GetListQualityBarcodeDto>> GetListQualitYBarcode(string vid);
    }
}