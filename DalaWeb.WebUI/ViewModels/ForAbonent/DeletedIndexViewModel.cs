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

    public class UploadFileStaticticsViewModel
    {
        public List<CounterStatistic> counterStatistic = new List<CounterStatistic>();

        public struct CounterStatistic
        {
            public double counterValue;
            public bool isSuccess;
            public string message;
            public string counterName;
            public CounterStatistic(string data_counterName, double data_counterValue, bool data_isSuccess, string data_message)
            {
                this.counterValue = data_counterValue;
                this.isSuccess = data_isSuccess;
                this.message = data_message;
                this.counterName = data_counterName;
            }
        }

        public UploadFileStaticticsViewModel(List<string> data_counterNamesList, List<double> data_counterValuesList, List<bool> data_isSuccessList, List<string> data_messagesList)
        {
            for (int i = 0; i < data_counterValuesList.Count; i++)
                this.counterStatistic.Add(new CounterStatistic(data_counterNamesList[i], data_counterValuesList[i], data_isSuccessList[i], data_messagesList[i]));            
        }

    }
}