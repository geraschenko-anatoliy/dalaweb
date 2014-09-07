using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Addresses;
using DalaWeb.Domain.Entities.Counters;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Payments;
using DalaWeb.Domain.Entities.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
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
        private Repository<Address> addressRepository;
        private Repository<City> cityRepository;
        private Repository<Street> streetRepository;
        private Repository<ServiceCompany> serviceCompanyRepository;
        private Repository<Service> serviceRepository;
        private Repository<AbonentService> abonentServiceRepository;

        private Repository<Counter> counterRepository;
        private Repository<CounterValues> counterValuesRepository;
        private Repository<Stamp> stampRepository;
        
        private Repository<Payment> paymentRepository;


        public IRepository<Payment> PaymentRepository
        {
            get
            {
                if (this.paymentRepository == null)
                    this.paymentRepository = new Repository<Payment>(context);
                return paymentRepository;
            }
        }

        public IRepository<Stamp> StampRepository
        {
            get
            {
                if (this.stampRepository == null)
                    this.stampRepository = new Repository<Stamp>(context);
                return stampRepository;
            }
        }

        public IRepository<CounterValues> CounterValuesRepository
        {
            get
            {
                if (this.counterValuesRepository == null)
                    this.counterValuesRepository = new Repository<CounterValues>(context);
                return counterValuesRepository;
            }
        }

        public IRepository<Counter> CounterRepository
        {
            get
            {
                if (this.counterRepository == null)
                    this.counterRepository = new Repository<Counter>(context);
                return counterRepository;
            }
        }

        public IRepository<AbonentService> AbonentServiceRepository
        {
            get
            {
                if (this.abonentServiceRepository == null)
                    this.abonentServiceRepository = new Repository<AbonentService>(context);
                return abonentServiceRepository;
            }
        }

        public IRepository<ServiceCompany> ServiceCompanyRepository
        {
            get
            {
                if (this.serviceCompanyRepository == null)
                    this.serviceCompanyRepository = new Repository<ServiceCompany>(context);
                return serviceCompanyRepository;
            }
        }
        public IRepository<Service> ServiceRepository
        {
            get
            {
                if (this.serviceRepository == null)
                    this.serviceRepository = new Repository<Service>(context);
                return serviceRepository;
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
        public IRepository<AbonentCredit> AbonentCreditRepository
        {
            get
            {
                if (this.abonentCreditRepository == null)
                    this.abonentCreditRepository = new Repository<AbonentCredit>(context);
                return abonentCreditRepository;
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

        public IRepository<Address> AddressRepository
        {
            get
            {
                if (this.addressRepository == null)
                    this.addressRepository = new Repository<Address>(context);
                return addressRepository;
            }
        }      
        public IRepository<City> CityRepository
        {
            get
            {
                if (this.cityRepository == null)
                    this.cityRepository = new Repository<City>(context);
                return cityRepository;
            }
        }  
        public IRepository<Street> StreetRepository
        {
            get
            {
                if (this.streetRepository == null)
                    this.streetRepository = new Repository<Street>(context);
                return streetRepository;
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
