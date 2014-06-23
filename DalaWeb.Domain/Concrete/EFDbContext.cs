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


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Entity<Abonent>()
            //    .HasMany(c => c.Credits).WithMany(i => i.Abonents)
            //    .Map(t => t.MapLeftKey("AbonentID")
            //        .MapRightKey("CreditID")
            //        .ToTable("AbonentCredit"));
        }


    }
}
