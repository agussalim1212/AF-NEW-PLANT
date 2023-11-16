using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Persistence.Repositories
{
    public class MaintCorrectiveRepository : IMaintCorrectiveRepository
    {
        public readonly IGenericRepository<MaintCorrective> _repository;

        public MaintCorrectiveRepository(IGenericRepository<MaintCorrective> repository)
        {
            _repository = repository;
        }

        public void DeleteMaintCorrective(MaintCorrective maintenanceCorrective) => _repository.DeleteAsync(maintenanceCorrective);

        public async Task<bool> ValidateData(MaintCorrective maintenanceCorrective)
        {
            var x = await _repository.Entities.Where(o => o.Id == maintenanceCorrective.Id).CountAsync();
            if (x > 0)
            {
                return false;
            }
            return true;
        }
    }
}
