using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Persistence.Repositories
{
    public class MachinesRepository : IMachinesRepository
    {
        private readonly IGenericRepository<Machine> _repository;

        public MachinesRepository(IGenericRepository<Machine> repository)
        {
            _repository = repository;
        }

        public void DeleteMachines(Machine machines) => _repository.DeleteAsync(machines);

        public async Task<bool> ValidateData(Machine machines)
        {
            var x = await _repository.Entities.Where(o => machines.Name.ToLower() == o.Name.ToLower()).CountAsync();
            if (x > 0)
            {
                return false;

            }
            return true;
        }

    }
}
