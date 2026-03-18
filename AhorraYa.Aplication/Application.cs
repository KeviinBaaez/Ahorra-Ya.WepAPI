using AhorraYa.Application.Interfaces;
using AhorraYa.Repository.Interfaces;
using System.Linq.Expressions;

namespace AhorraYa.Application
{
    public class Application<T> : IApplication<T>
    {
        private IGenericRepository<T> _repository;
        public Application(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public bool Exist(T entity)
        {
            return _repository.Exist(entity);
        }

        public bool Exist(Expression<Func<T, bool>> expression)
        {
            return (_repository.Exist(expression));
        }

        public IList<T> GetAll(Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Expression<Func<T, bool>>? filterByX1 = null,
            Expression<Func<T, bool>>? filterByX2 = null)
        {
            return _repository.GetAll(filter, orderBy, filterByX1, filterByX2);
        }

        public T GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void RemoveById(int id)
        {
            _repository.RemoveById(id);
        }

        public T Save(T entity)
        {
            return _repository.Save(entity);
        }
    }
}
