using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NetflixStatizier.Interfaces
{
    public interface IEntityRepositoryBase<T>
    {

        IQueryable<T> GetAll();

        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression);

        Task CreateAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);
    }
}
