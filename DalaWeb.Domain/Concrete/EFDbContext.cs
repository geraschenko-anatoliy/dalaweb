using DalaWeb.Domain.Entities;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Addresses;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalaWeb.Domain.Entities.Counters;
using System.Data.Metadata.Edm;
using DalaWeb.Domain.Entities.Payments;
using DalaWeb.Domain.Entities.PDFStorages;

namespace DalaWeb.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public DbSet<Abonent> Abonents { get; set; }
        public DbSet<Credit> Credits { get; set; }
        public DbSet<AbonentCredit> AbonentCredits { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ServiceCompany> ServiceCompanies { get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<Counter> Counters { get; set; }

        public DbSet<Stamp> Stamps { get; set; }

        public DbSet<CounterValues> CounterValues { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<PDFDocument> PDFDocuments { get; set; }

        public DbSet<Tariff> Tariffs { get; set; }

    }
}
