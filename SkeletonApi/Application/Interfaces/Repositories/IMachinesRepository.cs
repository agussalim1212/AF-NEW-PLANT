using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IMachinesRepository
    {

        Task<bool> ValidateData(Machine machines);

        void DeleteMachines(Machine machines);


    }
}
