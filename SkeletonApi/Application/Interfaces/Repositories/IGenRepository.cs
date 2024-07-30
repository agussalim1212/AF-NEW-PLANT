using System.Linq.Expressions;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IGenRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }

        Task<T> AddAsync(T entity);

        Task UpdateAsync(Guid machineId, Guid categoryId);

        Task DeleteAsync(T entity);

        Task<T> GetByIdAsync(Guid machineId, Guid categoryId);

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
    }
}