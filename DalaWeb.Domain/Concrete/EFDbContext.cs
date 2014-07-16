using DalaWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Entity<Abonent>()
            //             .HasMany(c => c.Services).WithMany(i => i.Abonents)
            //             .Map(t => t.MapLeftKey("AbonentId")
            //                 .MapRightKey("ServiceId")
            //                 .ToTable("AbonentsServices"));
        }


    }
}
