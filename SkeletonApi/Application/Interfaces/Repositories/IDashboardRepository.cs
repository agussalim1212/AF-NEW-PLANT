using SkeletonApi.Application.Features.Dashboard.FiveTopAirConsumption.Queries;
using SkeletonApi.Application.Features.Dashboard.FiveTopEnergyConsumption.Queries;
using SkeletonApi.Application.Features.Dashboard.FiveTopMachineMaintenance.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<GetAllTop5EnergyConsumptionsDto> GetAllTop5EnergyConsumptionsAsync();
        Task<GetAllTop5AirConsumptionsDto> GetAllTop5AirConsumptionsAsync();
        Task<GetAllTop5MachineMaintenanceDto> GetAllTop5MachineMaintenance();
    }
}
