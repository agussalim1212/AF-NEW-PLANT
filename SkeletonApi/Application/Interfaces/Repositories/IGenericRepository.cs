using SkeletonApi.Domain.Common.Interfaces;
using System.Linq.Expressions;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        IQueryable<T> Entities { get; }

        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByAsync(Guid id);
        Task UpdateByAsync(T entity, Guid machineId, Guid categoryId);
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task UpdateAsync(T entity, Guid id);
        Task DeleteAsync(T entity);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,bool trackChanges);
    }
}
