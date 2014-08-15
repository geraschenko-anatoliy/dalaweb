using DalaWeb.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Concrete
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal EFDbContext context;
        internal IDbSet<TEntity> dbSet;

        public Repository(EFDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>() ;
        }

        public virtual IQueryable<TEntity> Get()
        {
            IQueryable<TEntity> query = dbSet;
            return query.ToList().AsQueryable();
        }

        public virtual TEntity GetById(object id)
        {
            if (id is int[])
            {
                object[] objectArray = Array.ConvertAll<int, object>((int[])id, (x) => (object)x);
                return dbSet.Find(objectArray);
            }
            else
                return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            var entry = this.context.Entry(entityToUpdate);
            this.dbSet.Attach(entityToUpdate);
            entry.State = EntityState.Modified;
        }
    }
}
