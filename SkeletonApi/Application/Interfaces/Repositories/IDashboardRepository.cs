using SkeletonApi.Application.Features.Dashboard.FiveTopAirConsumption.Queries;
using SkeletonApi.Application.Features.Dashboard.FiveTopEnergyConsumption.Queries;
using SkeletonApi.Application.Features.Dashboard.FiveTopMachineMaintenance.Queries;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        //top 5 energy consumption
        Task<GetAllTop5EnergyConsumptionsDto> GetAllTop5EnergyConsumptionsAsync();
        //top 5 air consumption
        Task<GetAllTop5AirConsumptionsDto> GetAllTop5AirConsumptionsAsync();
        //top 5 machine maintenance
        Task<GetAllTop5MachineMaintenanceDto> GetAllTop5MachineMaintenance();
    }
}