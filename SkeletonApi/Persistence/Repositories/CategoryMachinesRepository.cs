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
    public class CategoryMachinesRepository : ICategoryMachineRepository
    {
        private readonly IGenericRepository<CategoryMachineHasMachine> _repository;

        public CategoryMachinesRepository(IGenericRepository<CategoryMachineHasMachine> repository)
        {
            _repository = repository;
        }

           
    }
}
