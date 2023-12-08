using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Persistence.Repositories
{
    public class MaintPreventiveRepository : IMaintenancesPreventive
    {
        private readonly IGenericRepository<MaintenacePreventive> _repository;
        public MaintPreventiveRepository(IGenericRepository<MaintenacePreventive> repository)
        {

            _repository = repository;

        }

        public void DeleteMachines(MaintenacePreventive maintenacePreventive) => _repository.DeleteAsync(maintenacePreventive);

        public async Task<bool> ValidateData(MaintenacePreventive maintenacePreventive)
        {
            var x = await _repository.Entities.Where(o => o.Id == maintenacePreventive.Id).CountAsync();
            if (x > 0)
            {
                return false;
            }
            return true;
        }
    }
}
