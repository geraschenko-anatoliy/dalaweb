using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Concrete;
using DalaWeb.Domain.Entities;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Addresses;
using DalaWeb.Domain.Entities.Counters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels.ForAbonent
{
    public class DeletedViewModel
    {
        public IQueryable<Street> streets;
        public IQueryable<City> cities;
        public IQueryable<Abonent> abonents;


        public DeletedViewModel(IRepository<Street> Streets, IRepository<City> Cities, IRepository<Abonent> Abonents)
        {
            this.streets = Streets.Get();
            this.cities = Cities.Get();
            this.abonents = Abonents.Get().Where(x => x.isDeleted == true);
        }
    }
}