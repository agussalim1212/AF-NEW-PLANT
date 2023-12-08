using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Persistence.Repositories
{
    public class DataRepository<T> : IDataRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        public DataRepository(ApplicationDbContext dbContext)
        {
          _dbContext = dbContext;
        }

        public IQueryable<T> Entities => _dbContext.Set<T>();

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }
      
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
        _dbContext.Set<T>()
        .Where(expression);

    }
}
