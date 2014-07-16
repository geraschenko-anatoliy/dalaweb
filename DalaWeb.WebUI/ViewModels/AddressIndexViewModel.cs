using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Concrete;
using DalaWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels
{
    public class AdressIndexViewModel
    {
        public IQueryable<Street> streets;
        public IQueryable<Address> addresses;
        public IQueryable<City> cities;

        public AdressIndexViewModel (IRepository<Street> Streets, IRepository<Address> Addresses, IRepository<City> Cities)
        {
            this.streets = Streets.Get();
            this.cities = Cities.Get();
            this.addresses = Addresses.Get();
        }
    }
}