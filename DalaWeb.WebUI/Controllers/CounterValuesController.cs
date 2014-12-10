using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DalaWeb.Domain.Entities.Counters;
using DalaWeb.Domain.Concrete;
using DalaWeb.Domain.Abstract;
using System.IO;
using System.Data.OleDb;
using System.Xml;
using System.Configuration;
using System.Data.SqlClient;
using DalaWeb.WebUI.ViewModels.ForAbonent;
using DalaWeb.WebUI.ViewModels.ForCounterValues;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;

namespace DalaWeb.WebUI.Controllers
{
    public class CounterValuesController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<CounterValues> counterValuesRepository;
        private IRepository<Counter> counterRepository;


        public CounterValuesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            counterValuesRepository = unitOfWork.CounterValuesRepository;
            counterRepository = unitOfWork.CounterRepository;
        }

        public ActionResult Index()
        {
            List<CounterValues> lastCounterValues = GetLastCounterValues();
            List<string> selectListItems = new List<string>();
            selectListItems.Add("Все");
            foreach(var item in unitOfWork.ServiceRepository.Get().Where(x => x.Type == 3).Where(x=>x.isOff==false).Select(x => x.Name).ToList())
            {
                selectListItems.Add(item);
            }
            ViewBag.ServiceName = new SelectList(selectListItems);
            return View(lastCounterValues);
        }

        List<CounterValues> GetLastCounterValues()
        {
            var counterValues = counterValuesRepository.Get().Include(c => c.Counter).Include(c => c.Counter.Service).Where(x=>x.Counter.Service.isOff == false);
            List<CounterValues> lastCounterValues = new List<CounterValues>();
            foreach (CounterValues cv in counterValues)
            {
                CounterValues temp = counterValues.Where(x => x.CounterId == cv.CounterId).Last();
                if (!lastCounterValues.Exists(x => x.CounterId == cv.CounterId))
                    lastCounterValues.Add(temp);
            }

            return lastCounterValues;
        }

        public ActionResult Details(int id = 0)
        {
            CounterValues countervalues = counterValuesRepository.GetById(id);

            if (countervalues == null)
            {
                return HttpNotFound();
            }
            return View(countervalues);
        }
        public ActionResult Create()
        {
            ViewBag.ServiceId = new SelectList(unitOfWork.ServiceRepository.Get().Where(x => x.isOff == false).Where(x => x.Type == 3), "ServiceId", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CounterValues countervalues)
        {
            if (ModelState.IsValid)
            {
                counterValuesRepository.Insert(countervalues);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name", countervalues.CounterId);
            return View(countervalues);
        }

        public ActionResult Edit(int id = 0)
        {
            CounterValues countervalues = counterValuesRepository.GetById(id);
            if (countervalues == null)
            {
                return HttpNotFound();
            }
            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name", countervalues.CounterId);
            return View(countervalues);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CounterValues countervalues)
        {
            if (ModelState.IsValid)
            {
                counterValuesRepository.Update(countervalues);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name", countervalues.CounterId);
            return View(countervalues);
        }
        public ActionResult Delete(int id = 0)
        {
            CounterValues countervalues = counterValuesRepository.GetById(id);
            if (countervalues == null)
            {
                return HttpNotFound();
            }
            return View(countervalues);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            counterValuesRepository.Delete(id);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(IEnumerable<HttpPostedFileBase> files, int serviceId)
        {
            List<string> dataCounterNamesList = new List<string>();
            List<double> dataCounterValuesList = new List<double>();
            List<bool> dataIsSuccessList = new List<bool>();
            List<string> dataMessagesList = new List<string>();


            foreach (var file in files)
            {
                if (file.ContentLength > 0)
                {
                    //string filePath = Path.Combine(HttpContext.Server.MapPath("../Uploads"),
                    //                               Path.GetFileName(file.FileName));
                    //string filePath = Path.Combine(HttpContext.Server.MapPath("/"),
                    //           Path.GetFileName(file.FileName));
                    //file.SaveAs(filePath);

                    var fileName = Path.GetFileName(file.FileName);

                    const int blockSize = 256;
                    int bytesNum;
                    byte[] buffer = new byte[blockSize];
                    MemoryStream ms = new MemoryStream();

                    while ((bytesNum = file.InputStream.Read(buffer, 0, blockSize)) > 0)
                        ms.Write(buffer, 0, bytesNum);

                    Stream ImportStream = ms;
                    DataTable dt =  getDataTableFromExcel(ImportStream);

                    IQueryable<Counter> counters = counterRepository.Get().Include(c => c.CounterValues);
                    List<CounterValues> lastCounterValues = GetLastCounterValues();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        InsertCounterValueToRepository(dataCounterNamesList,
                            dataCounterValuesList,
                            dataIsSuccessList,
                            dataMessagesList,
                            lastCounterValues,
                            counters,
                            serviceId,
                            TryToParseInt(dt.Rows[i].ItemArray[0].ToString()).ToString(),
                            TryToParseDouble(dt.Rows[i].ItemArray[2].ToString()),
                            TryToParseDateTime(dt.Rows[i].ItemArray[1].ToString()));
                    }

                    unitOfWork.Save();
                }
            }
            
            return View(new UploadFileStaticticsViewModel(dataCounterNamesList, dataCounterValuesList, dataIsSuccessList, dataMessagesList));
        }



        public static DataTable getDataTableFromExcel(Stream stream)
        {
            DataTable result = new DataTable();
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                if (stream != null)
                    pck.Load(stream);

                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                for (int i = 0; i < 8; i++)
                    tbl.Columns.Add("");

                var startRow = 6;
                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    var row = tbl.NewRow();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Value; //cell.text
                    }
                    tbl.Rows.Add(row);
                }
                result = tbl;
            }
            return result;
        }



        private void InsertCounterValueToRepository(List<string> data_counterNamesList,
            List<double> data_counterValuesList,
            List<bool> data_isSuccessList,
            List<string> data_messagesList,
            List<CounterValues> lastCounterValues,
            IQueryable<Counter> counters,
            int serviceId,
            string counterName,
            double value,
            DateTime date)
        {
            var counterList = counters.Where(x => x.Name == counterName)
                                                .Where(x => x.isOff == false)
                                                .Where(x => x.ServiceId == serviceId);

            if (counterList.Any())
            {
                Counter counter = counterList.First();
                if (counter.CounterValues.Where(x => x.Value == value).Where(x => x.Date == date).Any())
                {
                    AddWithError(data_counterNamesList, data_counterValuesList, data_isSuccessList, data_messagesList,
                                counterName, value, "Такие показания есть в базе");
                    return;
                }
                if (counter.CounterValues.Last().Value >= value)
                {
                    AddWithError(data_counterNamesList, data_counterValuesList, data_isSuccessList, data_messagesList,
                                counterName, value, "Текущие показания меньше или равны предыдущим");
                    return;
                }

                if (counter.CounterValues.Last().Date > date)
                {
                    AddWithError(data_counterNamesList, data_counterValuesList, data_isSuccessList, data_messagesList,
                                counterName, value, "Импортируемая дата меньше существующей в базе " + date.ToString() );
                    return;
                }

                if (counter.CounterValues.Where(x => x.Date == date).Any())
                {
                    AddWithError(data_counterNamesList, data_counterValuesList, data_isSuccessList, data_messagesList,
                                counterName, value, string.Concat("Показания счетчика с датой ", date.ToShortDateString(), " уже есть в базе"));
                    return;
                }

                CounterValues counterValue = new CounterValues
                            {
                                CounterId = counter.CounterId,
                                Value = value,
                                Date = date,
                                Counter = counter
                            };

                counterValuesRepository.Insert(counterValue);

                data_counterNamesList.Add(counterName);
                data_counterValuesList.Add(value);
                data_isSuccessList.Add(true);
                data_messagesList.Add("");
            }
            else
            {
                AddWithError(data_counterNamesList, data_counterValuesList, data_isSuccessList, data_messagesList,
                            counterName, value, "Счетчик не найден");
            }
        }

        private void AddWithError(List<string> data_counterNamesList,
            List<double> data_counterValuesList,
            List<bool> data_isSuccessList,
            List<string> data_messagesList,
            string counterName,
            double value,
            string error)
        {
            data_counterNamesList.Add(counterName);
            data_counterValuesList.Add(value);
            data_isSuccessList.Add(false);
            data_messagesList.Add(error);
        }

        private static int TryToParseInt(string value)
        {
            int number;
            bool result = Int32.TryParse(value, out number);
            if (result)
            {
                return number;
            }
            else
            {
                if (value == null)
                    return 0;
                else
                    return -1;
            }
        }
        private static double TryToParseDouble(string value)
        {
            double number;
            bool result = double.TryParse(value, out number);
            if (result)
            {
                return number;
            }
            else
            {
                if (value == null)
                    return 0;
                else
                    return -1;
            }
        }
        private static DateTime TryToParseDateTime(string value)
        {
            DateTime date;
            if (DateTime.TryParse(value, out date))
            {
                return date;
            }
            else
            {
                if (value == null)
                    return DateTime.MinValue;
                else
                    return DateTime.MinValue;
            }

        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}