using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetflixStatizier.Interfaces;

namespace NetflixStatizier.Data.Repositories
{
    public class EntityRepositoryBase<T> : IEntityRepositoryBase<T> where T : class
    {
        private readonly StatsContext m_Context;

        public EntityRepositoryBase(StatsContext context)
        {
            m_Context = context;
        }

        public IQueryable<T> GetAll()
        {
            return m_Context.Set<T>();
        }

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression)
        {
            return m_Context.Set<T>()
                .Where(expression);
        }

        public async Task CreateAsync(T entity)
        {
            await m_Context.Set<T>()
                .AddAsync(entity);
            await m_Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            m_Context.Set<T>()
                .Update(entity);
            await m_Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            m_Context.Set<T>()
                .Remove(entity);
            await m_Context.SaveChangesAsync();
        }
    }
}
