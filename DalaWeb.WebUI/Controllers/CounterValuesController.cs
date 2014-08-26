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
using DalaWeb.WebUI.ViewModels;

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
            ViewBag.ServiceName = new SelectList(unitOfWork.ServiceRepository.Get().Where(x => x.Type == 3).Select(x => x.Name));
            return View(lastCounterValues);
        }

        List<CounterValues> GetLastCounterValues()
        {
            var counterValues = counterValuesRepository.Get().Include(c => c.Counter).Include(c => c.Counter.Service);
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
            ViewBag.ServiceId = new SelectList(unitOfWork.ServiceRepository.Get().Where(x => x.Type == 3), "ServiceId", "Name");
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
                DataSet ds = new DataSet();
                if (Request.Files["files"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["files"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = Server.MapPath("~/Content/") + Request.Files["files"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        Request.Files["files"].SaveAs(fileLocation);
                        string excelConnectionString = string.Empty;

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0 Macro;HDR=Yes;IMEX=2\"";
                        if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0 Macro;HDR=Yes;IMEX=2\"";
                        }

                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();


                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        //if (dt == null)
                        //{
                        //    //return View();
                        //    return;
                        //}

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }

                        excelConnection.Close();
                    }

                    //for xml files
                    //if (fileExtension.ToString().ToLower().Equals(".xml"))
                    //{
                    //    string fileLocation = Server.MapPath("~/Content/") + Request.Files["FileUpload"].FileName;
                    //    if (System.IO.File.Exists(fileLocation))
                    //    {
                    //        System.IO.File.Delete(fileLocation);
                    //    }

                    //    Request.Files["FileUpload"].SaveAs(fileLocation);
                    //    XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                    //    // DataSet ds = new DataSet();
                    //    ds.ReadXml(xmlreader);
                    //    xmlreader.Close();
                    //}

                    IQueryable<Counter> counters = counterRepository.Get().Include(c => c.CounterValues);
                    List<CounterValues> lastCounterValues = GetLastCounterValues();

                    for (int i = 4; i < ds.Tables[0].Rows.Count - 5; i++)
                    {
                        InsertCounterValueToRepository(dataCounterNamesList,
                            dataCounterValuesList,
                            dataIsSuccessList,
                            dataMessagesList,
                            lastCounterValues,
                            counters,
                            serviceId,
                            TryToParseInt(ds.Tables[0].Rows[i][0].ToString()).ToString(),
                            TryToParseDouble(ds.Tables[0].Rows[i][2].ToString()),
                            TryToParseDateTime(ds.Tables[0].Rows[i][1].ToString()));
                    }

                    unitOfWork.Save();
                }
            }
            return View(new UploadFileStaticticsViewModel(dataCounterNamesList, dataCounterValuesList, dataIsSuccessList, dataMessagesList));
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
                                counterName, value, "Текущие показания меньше предыдущих");
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
            bool result = DateTime.TryParse(value, out date);
            if (result)
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