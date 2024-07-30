using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IStatusMachineRepository
    {
        Task<IEnumerable<Machine>> GetAllMachinesAsync();
    }
}