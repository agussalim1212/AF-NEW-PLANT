using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Persistence.Repositories
{
    public class StatusMachineRepository : IStatusMachineRepository
    {
        private readonly IGenericRepository<Machine> _RepoMachine;

        public StatusMachineRepository(IGenericRepository<Machine> RepoMachine)
        {
            _RepoMachine = RepoMachine;
        }

       public async Task<IEnumerable<Machine>> GetAllMachinesAsync() => await _RepoMachine.GetAllAsync();
       
    }
}
