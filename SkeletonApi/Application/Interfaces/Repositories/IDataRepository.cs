using System.Linq.Expressions;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDataRepository<T>
    {
        IQueryable<T> Entities { get; }

        Task<T> AddAsync(T entity);

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
    }
}