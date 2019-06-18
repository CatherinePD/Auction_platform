using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Auction.DataAccess
{
    public class AuctionRepository<T> : IRepository<T> where T : class
    {
        private readonly AuctionContext _context;
        private readonly DbSet<T> _dbSet;

        public AuctionRepository(AuctionContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public AuctionRepository()
        {
            _context = new AuctionContext();
            _dbSet = _context.Set<T>();
        }

        public T Get(int id)
        {
            return _dbSet.Find(id);
        }

        public ICollection<T> Get()
        {
            return _dbSet.ToList();
        }

        public ICollection<T> Get(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public IQueryable<T> Select()
        {
            return _dbSet;
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }
    }
}
