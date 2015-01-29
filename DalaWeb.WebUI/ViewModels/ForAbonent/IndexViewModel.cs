using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Concrete;
using DalaWeb.Domain.Entities;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels.ForAbonent
{
    public class IndexViewModel
    {
        public IQueryable<Street> streets;
        public IQueryable<City> cities;
        public IQueryable<Abonent> abonents;

        public IndexViewModel(IQueryable<Street> streets, IQueryable<City> cities, IQueryable<Abonent> abonent)
        {
            this.streets = streets;
            this.cities = cities;
            this.abonents = abonent;
        }
    }
}