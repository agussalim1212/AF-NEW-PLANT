using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IDataRepository<T>
    {
        IQueryable<T> Entities { get; }
        Task<T> AddAsync(T entity);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
    }
}
