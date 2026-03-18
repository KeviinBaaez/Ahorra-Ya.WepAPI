using AhorraYa.Abstractions;
using AhorraYa.Repository.Interfaces;
using System.Linq.Expressions;

namespace AhorraYa.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected IDbContext<T> _dbContext;

        public GenericRepository(IDbContext<T> dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Exist(T entity)
        {
            return _dbContext.Exist(entity);
        }

        public bool Exist(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Exist(expression);
        }

        public IList<T> GetAll(Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Expression<Func<T, bool>>? filterByX1 = null,
            Expression<Func<T, bool>>? filterByX2 = null)
        {
            return _dbContext.GetAll(filter, orderBy, filterByX1, filterByX2);
        }

        public T GetById(int id)
        {
            return _dbContext.GetById(id);
        }

        public void RemoveById(int id)
        {
            _dbContext.RemoveById(id);
        }

        public T Save(T entity)
        {
            return _dbContext.Save(entity);
        }
    }
}
