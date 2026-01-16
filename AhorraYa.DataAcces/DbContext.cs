using AhorraYa.Abstractions;
using AhorraYa.Exceptions;
using AhorraYa.Exceptions.ExceptionsForId;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AhorraYa.DataAccess
{
    public class DbContext<T> : IDbContext<T> where T : class, IEntidad
    {
        DbSet<T> _Items;
        DbDataAccess _ctx;
        public DbContext(DbDataAccess ctx)
        {
            _ctx = ctx;
            _Items = ctx.Set<T>();
        }

        public void RemoveById(int id)
        {
            _Items.Remove(_Items.FirstOrDefault(e => e.Id == id)!);
            _ctx.SaveChanges();
        }

        public IList<T> GetAll(Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = _Items.AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query.AsNoTracking().ToList();
        }

        public T GetById(int id)
        {
            if (id <= 0)
            {
                throw new ExceptionIdNotZero();
            }
            var entity = _Items.FirstOrDefault(i => i.Id == id)!;
            if (entity is null)
            {
                throw new ExceptionIdNotFound(typeof(T), id.ToString());
            }
            return entity;
        }



        public T Save(T entity)
        {
            if (entity.Id == 0)
            {
                if (!Exist(entity))
                {
                    _Items.Add(entity);
                }
            }
            else
            {
                var entityDb = GetById(entity.Id);
                if (entityDb is null)
                {
                    throw new ExceptionIdNotFound(typeof(T), entity.Id.ToString());
                }
                if (!Exist(entity))
                {
                    _ctx.Entry(entityDb).State = EntityState.Detached;
                    _Items.Update(entity);
                }
            }
            _ctx.SaveChanges();
            return entity;
        }

        public bool Exist(T entity)
        {

            if (_Items.ToList().Any(i => i.Equals(entity))) //Validación que no existan 2 obj iguales
            {
                throw new ExceptionAlreadyExist(typeof(T));
            }

            return false;
        }

        public bool Exist(Expression<Func<T, bool>> expression)
        {
            if (_Items.Any(expression))
            {
                throw new ExceptionAlreadyExist(typeof(T));
            }
            return false;
        }
    }
}
