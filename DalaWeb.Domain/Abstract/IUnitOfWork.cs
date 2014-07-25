﻿using DalaWeb.Domain.Entities;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Addresses;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Services;
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

        IRepository<Address> AddressRepository { get; }
        IRepository<City> CityRepository { get; }
        IRepository<Street> StreetRepository { get; }


        IRepository<AbonentService> AbonentServiceRepository { get; }
        IRepository<Service> ServiceRepository { get; }
        IRepository<ServiceCompany> ServiceCompanyRepository { get; }
    }
}
