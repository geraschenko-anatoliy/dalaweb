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
            //return query.ToList().AsQueryable().AsNoTracking();
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
            //dbSet.Attach(entityToUpdate);
            ////context.Entry().CurrentValues.SetValues(entityToUpdate);
            ////db.Entry(v).CurrentValues.SetValues(model);
            //context.Entry(entityToUpdate).State = EntityState.Modified;

            var entry = this.context.Entry(entityToUpdate);
            this.dbSet.Attach(entityToUpdate);
            entry.State = EntityState.Modified;
        }

        //public virtual void Update(TEntity entity) 
        //{
        //    if (entity == null) {
        //        throw new ArgumentException("Cannot add a null entity.");
        //    }

        //    var entry = context.Entry<TEntity>(entity);

        //    if (entry.State == EntityState.Detached) {
        //        var set = context.Set<TEntity>();
        //        TEntity attachedEntity = set.Local.SingleOrDefault(e => e == entity);  // You need to have access to key

        //        if (attachedEntity != null) {
        //            var attachedEntry = context.Entry(attachedEntity);
        //            attachedEntry.CurrentValues.SetValues(entity);
        //        } else {
        //            entry.State = EntityState.Modified; // This should attach entity
        //        }
        //    }
        //}
    }
}
