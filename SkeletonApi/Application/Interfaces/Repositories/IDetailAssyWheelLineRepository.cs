using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.AirConsumptionAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.EnergyConsumptionAssyWheelLine;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.MachineInformationAssyWheelLine;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDetailAssyWheelLineRepository
    {
        Task<GetAllMachineInformationAssyWheelLineDto> GetAllMachineInformationAsync(Guid machine_id);
        Task<GetAllEnergyConsumptionAssyWheelLineDto> GetAllEnergyConsumption(Guid machine_id, string type, DateTime start, DateTime end);
        Task<GetAllAirConsumptionAssyWheelLineDto> GetAllAirConsumption(Guid machine_id, string type, DateTime start, DateTime end);
    }
}
