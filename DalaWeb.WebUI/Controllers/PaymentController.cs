	using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DalaWeb.Domain.Entities.Payments;
using DalaWeb.Domain.Concrete;
using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Services;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Counters;
using System.Globalization;
using DalaWeb.WebUI.ViewModels.ForPayment;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DalaWeb.WebUI.Infrastructure;

namespace DalaWeb.WebUI.Controllers
{
	[Authorize]
	public class PaymentController : Controller
	{
		private IUnitOfWork unitOfWork;
		private IRepository<Payment> paymentRepository;
		private IRepository<Abonent> abonentRepository;
		private IRepository<AbonentCredit> abonentCreditRepository;
		private IRepository<AbonentService> abonentServiceRepository;
		private IRepository<Counter> counterRepository;
		private IRepository<CounterValues> counterValuesRepository;
		
		public PaymentController(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
			paymentRepository = unitOfWork.PaymentRepository;
			abonentRepository = unitOfWork.AbonentRepository;
			abonentCreditRepository = unitOfWork.AbonentCreditRepository;
			abonentServiceRepository = unitOfWork.AbonentServiceRepository;
			counterRepository = unitOfWork.CounterRepository;
			counterValuesRepository = unitOfWork.CounterValuesRepository;
		}

		public ActionResult Index()
		{
			ViewBag.AbonentNumber = AddAllToList(abonentRepository.Get()
				.OrderBy(x=>x.AbonentNumber)
				.Select(x => x.AbonentNumber)
				.Where(x => (!string.IsNullOrEmpty(x))));

			ViewBag.Year = AddAllToList(paymentRepository.Get().Select(x => x.Date.Year.ToString()).Distinct());
			ViewBag.Month = GetMonthsToDropDown();
			List<string> paymentTypes = new List<string>();
			paymentTypes.Add("Пополнение");
			paymentTypes.Add("Списание");
			ViewBag.PaymentType = AddAllToList(paymentTypes.AsQueryable());

			return View(paymentRepository.Get());
		}
		public ActionResult Receipts()
		{
			ViewBag.AbonentNumber = AddAllToList(abonentRepository.Get()
				.OrderBy(x => x.AbonentNumber)
				.Select(x => x.AbonentNumber)
				.Where(x => (!string.IsNullOrEmpty(x))));

			ViewBag.Year = AddAllToList(paymentRepository.Get().Select(x => x.Date.Year.ToString()).Distinct());
			ViewBag.Month = GetMonthsToDropDown();
			return View(new IndexViewModel(paymentRepository.Get()));
		}
		public ActionResult ReceiptForAbonent(int abonentId, int year, string month)
		{
			DateTime date =  DateTime.Parse("1 " + month + " " + year.ToString());
			var monthPayments = paymentRepository.Get().Where(x => x.Abonent.AbonentId == abonentId)
														.Where(x=>x.Date < date.AddMonths(1));
			ViewBag.date = date;

			return View(monthPayments);
		}
		public ActionResult RecalculateMonth(int abonentId, int year, string month)
		{
			DateTime date = DateTime.Parse("1 "+month + " " + year.ToString());

			Abonent abonent = abonentRepository.GetById(abonentId);

			MonthCalculator mCalc = new MonthCalculator(unitOfWork);
			List<string> warnings = mCalc.ReCalculateAbonent(abonent, date);

			PDFAbonentMonthlyReceiptGenerator pg = new PDFAbonentMonthlyReceiptGenerator(unitOfWork, abonent, date, Server.MapPath("~/Content/fonts/ARIAL.ttf"));
			pg.GenerateAbonentPDFReceipt();
		   
			IQueryable<Payment> payments = paymentRepository.Get();
			payments = payments
				.Where(x => x.Date == date)
				.Where(x => x.Abonent.AbonentId == abonentId);
			return View("RecalculateMonthSummary", new RecalculateMonthSummaryViewModel(warnings, payments.ToList()));
		}
		public PartialViewResult Filter(string abonentNumber, string Year, int? Month, string paymentType)
		{
			IQueryable<Payment> payments = paymentRepository.Get();

			if (!string.IsNullOrEmpty(abonentNumber) && abonentNumber != "Все")
				payments = payments.Where(x => x.Abonent.AbonentNumber == abonentNumber);
			if (!string.IsNullOrEmpty(Year) && Year != "Все")
				payments = payments.Where(x => x.Date.Year == TryToParseInt(Year));
			if (Month.HasValue && Month != 0)
				payments = payments.Where(x => x.Date.Month == Month.Value);
			if (!string.IsNullOrEmpty(paymentType) && paymentType != "Все")
				if (paymentType == "Списание")
					payments = payments.Where(x => (x.AbonentCredit != null || x.AbonentService != null));
				else
					payments = payments.Where(x => (x.AbonentCredit == null || x.AbonentService == null));

			return PartialView(payments.OrderBy(x => x.Date));
		}
		public PartialViewResult ReceiptFilter(string abonentNumber, string Year, int? Month, string paymentType)
		{
			IQueryable<Payment> payments = paymentRepository.Get();

			if (!string.IsNullOrEmpty(abonentNumber) && abonentNumber != "Все")
				payments = payments.Where(x => x.Abonent.AbonentNumber == abonentNumber);
			if (!string.IsNullOrEmpty(Year) && Year != "Все")
				payments = payments.Where(x => x.Date.Year == TryToParseInt(Year));
			if (Month.HasValue && Month != 0)
				payments = payments.Where(x => x.Date.Month == Month.Value);
			if (!string.IsNullOrEmpty(paymentType) && paymentType != "Все")
				if (paymentType == "Списание")
					payments = payments.Where(x => (x.AbonentCredit != null || x.AbonentService != null));
				else
					payments = payments.Where(x => (x.AbonentCredit == null || x.AbonentService == null));

			return PartialView(new IndexViewModel(payments));
		}
		public ActionResult Details(int id = 0)
		{
			Payment payment = paymentRepository.GetById(id);
			if (payment == null)
			{
				return HttpNotFound();
			}
			return View(payment);
		}
		public ActionResult OpenMonth()
		{
			List<string> errors = MonthCalculator.isPossibleToOpenMonths(paymentRepository, abonentRepository);

			foreach (string error in errors)
			{
				ModelState.AddModelError(String.Empty, error);
				ViewBag.HasErrors = true;
				return View();
			}
			ViewBag.HasErrors = false;
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult OpenMonth(DateTime date)
		{
			MonthCalculator mCalc = new MonthCalculator(unitOfWork);
			var a = counterRepository.Get();
			List<string> warnings = mCalc.CalculateAllAbonents(date);



			foreach(var abonent in abonentRepository.Get().Where(x=>x.isDeleted == false))
			{
				PDFAbonentMonthlyReceiptGenerator pg = new PDFAbonentMonthlyReceiptGenerator(unitOfWork, abonent, date, Server.MapPath("~/Content/fonts/ARIAL.ttf"));
				pg.GenerateAbonentPDFReceipt();
			}

			var payments = paymentRepository.Get().Where(x => x.Date == date);

			return View("OpenMonthSummary", new OpenMonthSummaryViewModel(warnings, payments.ToList()));
		}
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Payment payment, string AbonentNumber)
		{
			if (!abonentRepository.Get().Where(x => x.AbonentNumber == AbonentNumber).Any())
				ModelState.AddModelError("AbonentNumber", "Абонент не найден");
			else
			{
				payment.Abonent = abonentRepository.Get().Where(x => x.AbonentNumber == AbonentNumber)
														   .Where(x => x.isDeleted == false)
														   .LastOrDefault();
			}
			if (ModelState.IsValid)
			{
				paymentRepository.Insert(payment);
				unitOfWork.Save();
				return RedirectToAction("Index");
			}
			return View(payment);
		}
		
		public ActionResult CreateForAbonent(int abonentId)
		{
            ViewBag.AbonentId = abonentId;
            ViewBag.AbonentName = abonentRepository.GetById(abonentId).Name;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateForAbonent(Payment payment)
		{
            if (ModelState.IsValid)
            {
                paymentRepository.Insert(payment);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.AbonentId = payment.AbonentId;
            ViewBag.AbonentName = abonentRepository.GetById(payment.AbonentId).Name;
            return View();
		}

		public FileStreamResult GetPDF(int abonentId, int year, string month)
		{
			Domain.Entities.PDFStorages.PDFAbonentMonthlyReceipt doc = unitOfWork.PDFAbonentMonthlyReceiptRepository.Get()
				.Where(x => x.Abonent.AbonentId == abonentId)
				.Where(x => x.Date.ToString("MMMM") == month)
				.Where(x => x.Date.Year == year)
				.LastOrDefault();

			MemoryStream workStream = new MemoryStream(doc.Value);
			workStream.Position = 0;

			return new FileStreamResult(workStream, "application/pdf");
		}
		public ActionResult NumerousRefill()
		{
			var abonents = abonentRepository.Get()
				.Where(x => x.isDeleted == false)
				.ToList();

			ViewBag.Abonents = abonents;
			List<NumerousViewModel> abonentModels = new List<NumerousViewModel>();

			foreach (var item in abonents)
			{
				abonentModels.Add(new NumerousViewModel(item.AbonentNumber, item.Name, item.AbonentId, item.Payments.Any() ? item.Payments.Sum(x=>x.Sum) : 0));
			}

			return View(abonentModels);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult NumerousRefill(List<NumerousViewModel> Numerous, DateTime date)
		{
			foreach (var item in Numerous)
			{
				if (item.Sum != 0)
				{
					Payment payment = new Payment()
					{
						Abonent = abonentRepository.GetById(item.AbonentId),
						Comment = "Пополнение счета за " + date.ToString("D"), 
						Date = date,
						Sum = item.Sum
					};
					paymentRepository.Insert(payment);
				}
			}
			unitOfWork.Save();
			return RedirectToAction("Index");
		}
		public JsonResult AutoCompleteAbonentNumber(string term)
			{
			var result = (from r in abonentRepository.Get()
						  where r.AbonentNumber.ToLower().Contains(term.ToLower())
						  select new { r.AbonentNumber }).Distinct();
			return Json(result, JsonRequestBehavior.AllowGet);
		}
		public ActionResult Delete(int id = 0)
		{
			Payment payment = paymentRepository.GetById(id);
			if (payment == null)
			{
				return HttpNotFound();
			}
			return View(payment);
		}
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			paymentRepository.Delete(id);
			unitOfWork.Save();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			unitOfWork.Dispose();
			base.Dispose(disposing);
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
		private SelectList GetMonthsToDropDown()
		{
			List<SelectListItem> result = new List<SelectListItem>();

			SelectListItem all = new SelectListItem()
			{
				Text = "Все",
				Value = "0"
			};

			result.Add(all);

			DateTime date = DateTime.MinValue;

			for (int i = 1; i<13; i++)
			{
				SelectListItem currentMonth = new SelectListItem()
				{
					Text = date.ToString("MMMM"),
					Value = i.ToString()
				};
				result.Add(currentMonth);
				date = date.AddMonths(1);
			}

			return new SelectList(result, "Value", "Text");
		}
		private SelectList AddAllToList(IQueryable<string> IQueryable)
		{
			List<string> selectListItems = new List<string>();
			selectListItems.Add("Все");
			foreach (var item in IQueryable.ToList())
			{
				selectListItems.Add(item);
			}
			return new SelectList (selectListItems);
		}
	}
}