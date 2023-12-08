using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IMaintenancesPreventive
    {
        Task<bool> ValidateData(MaintenacePreventive maintenacePreventive);

        void DeleteMachines(MaintenacePreventive maintenacePreventive);
    }
}
