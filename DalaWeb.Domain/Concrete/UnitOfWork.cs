using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private EFDbContext context = new EFDbContext();
        private Repository<Abonent> abonentRepository;
        private Repository<Credit> creditRepository;
        private Repository<AbonentCredit> abonentCreditRepository;

        public IRepository<AbonentCredit> AbonentCreditRepository
        {
            get
            {
                if (this.abonentCreditRepository == null)
                    this.abonentCreditRepository = new Repository<AbonentCredit>(context);
                return abonentCreditRepository;
            }
        }
        public IRepository<Abonent> AbonentRepository
        {
            get
            {
                if (this.abonentRepository == null)
                    this.abonentRepository = new Repository<Abonent>(context);
                return abonentRepository;
            }
        }
        public IRepository<Credit> CreditRepository
        {
            get
            {
                if (this.creditRepository == null)
                    this.creditRepository = new Repository<Credit>(context);
                return creditRepository;
            }
        }
        public void Save()
        {
            context.SaveChanges();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
