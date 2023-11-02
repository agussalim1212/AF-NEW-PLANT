using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SkeletonApi.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseAuditableEntity
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> Entities => _dbContext.Set<T>();

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task UpdateAsync(T entity)
        {
            T exist = _dbContext.Set<T>().Find(entity);
            _dbContext.Entry(exist).CurrentValues.SetValues(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext
                .Set<T>()
                .ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> GetByAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);

        }

        public Task UpdateByAsync(T entity,Guid machineId, Guid categoryId)
        {
            T exist = _dbContext.Set<T>().Find(machineId,categoryId);
            _dbContext.Entry(exist).CurrentValues.SetValues(exist);
            return Task.CompletedTask;
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,bool trackChanges) =>
       !trackChanges ?
        _dbContext.Set<T>()
           .Where(expression)
           .AsNoTracking() :
         _dbContext.Set<T>()
           .Where(expression);
    }
}
