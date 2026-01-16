using System.Linq.Expressions;

namespace AhorraYa.Abstractions
{
    public interface IDbOperation<T>
    {
        T Save (T entity);
        IList<T> GetAll (Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        T GetById (int id);
        void RemoveById (int id);
        bool Exist(T entity);
        bool Exist(Expression<Func<T, bool>> expression);
    }
}
