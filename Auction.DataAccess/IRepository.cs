using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Auction.DataAccess
{
    public interface IRepository<T>
    {
        T Get(int id);

        ICollection<T> Get();

        ICollection<T> Get(Expression<Func<T, bool>> predicate);

        IQueryable<T> Select();

        void Update(T entity);

        void Delete(T entity);

        void Add(T entity);
    }
}