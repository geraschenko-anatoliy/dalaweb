using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels.ForCounterValues
{
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