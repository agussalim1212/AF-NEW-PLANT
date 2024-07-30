using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Persistence.Contexts;
using System.Linq.Expressions;

namespace SkeletonApi.Persistence.Repositories
{
    public class GenRepository<T> : IGenRepository<T> where T : BaseManyToMany
    {
        private readonly ApplicationDbContext _dbContext;

        public GenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> Entities => _dbContext.Set<T>();

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task UpdateAsync(Guid machineId, Guid categoryId)
        {
            T exist = _dbContext.Set<T>().Find(machineId, categoryId);
            _dbContext.Entry(exist).CurrentValues.SetValues(exist);
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

        public async Task<T> GetByIdAsync(Guid machineId, Guid categoryId)
        {
            return await _dbContext.Set<T>().FindAsync(machineId, categoryId);
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
           _dbContext.Set<T>()
           .Where(expression);
    }
}