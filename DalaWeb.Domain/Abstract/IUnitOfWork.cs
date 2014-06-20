using DalaWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        IRepository<Abonent> AbonentRepository { get; }
        IRepository<Credit> CreditRepository { get; }
        IRepository<AbonentCredit> AbonentCreditRepository { get; }
    }
}
