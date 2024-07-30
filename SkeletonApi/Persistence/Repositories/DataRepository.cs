using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Persistence.Contexts;
using System.Linq.Expressions;

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