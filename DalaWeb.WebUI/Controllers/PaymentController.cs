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

namespace DalaWeb.WebUI.Controllers
{
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
			ViewBag.AbonentNumber = AddAllToList(abonentRepository.Get().Select(x => x.AbonentNumber));
			ViewBag.Year = AddAllToList(paymentRepository.Get().Select(x => x.Date.Year.ToString()).Distinct());
			ViewBag.Month = GetMonthsToDropDown(); //AddMonthToList(paymentRepository.Get().Select(x => x.Date).Distinct());
			ViewBag.PaymentType = AddAllToList(paymentRepository.Get().Select(x => x.Type).Distinct());

			var payments = paymentRepository.Get().Include(p => p.Abonent);

			return View(payments);
		}

		public ActionResult Month()
		{
			ViewBag.AbonentNumber = AddAllToList(abonentRepository.Get().Select(x => x.AbonentNumber).Where(x=> (!string.IsNullOrEmpty(x))));
			ViewBag.Year = AddAllToList(paymentRepository.Get().Select(x => x.Date.Year.ToString()).Distinct());
			ViewBag.Month = GetMonthsToDropDown();
			ViewBag.PaymentType = AddAllToList(paymentRepository.Get().Select(x => x.Type).Distinct());
			return View(new IndexViewModel(paymentRepository.Get()));
		}

		SelectList GetMonthsToDropDown()
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
					Text = date.ToString("MMMM", CultureInfo.CreateSpecificCulture("ru-RU")),
					Value = i.ToString()
				};
				result.Add(currentMonth);
				date = date.AddMonths(1);
			}

			return new SelectList(result, "Value", "Text");
		}

		SelectList AddAllToList(IQueryable<string> IQueryable)
		{
			List<string> selectListItems = new List<string>();
			selectListItems.Add("Все");
			foreach (var item in IQueryable.ToList())
			{
				selectListItems.Add(item);
			}
			return new SelectList (selectListItems);
		}

		public ActionResult ForAbonent(int abonentId, int year, string month)
		{
			//var monthPayments = paymentRepository.Get().Where(x => x.AbonentId == abonentId)
			//                                            .Where(x => x.Date.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU")) == month)
			//                                            .Where(x => x.Date.Year == year)
			//                                            .OrderBy(x => x.Date)
			//                                            .OrderBy(x => x.PaymentId);

			DateTime date = new DateTime(2015, 1, 1);

			var monthPayments = paymentRepository.Get().Where(x => x.AbonentId == abonentId)
											.Where(x => x.Date < date)
											.OrderBy(x => x.Date)
											.OrderBy(x => x.PaymentId);

			return View(monthPayments);
		}      
		
		public ActionResult RecalculateMonth(int abonentId, int year, string month)
		{
			var monthPayments =  paymentRepository.Get().Where(x => x.AbonentId == abonentId)
														.Where(x => x.Date.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU")) == month)
														.Where(x => x.Date.Year == year)
														.Where(x=>x.Type == "Списание");
			DateTime date;
			if (monthPayments.Any())
				date = monthPayments.First().Date;
			else
				date = paymentRepository.Get().Where(x => x.Date.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU")) == month)
												.Where(x => x.Date.Year == year)
												.Where(x=>x.Type == "Списание")
												.First()
												.Date;

			foreach(var payment in monthPayments)
			{
				paymentRepository.Delete(payment);
			}

			var fullyPaidCredits = abonentCreditRepository.Get()
				.Where(x => x.FinishDate.Month.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU")) == month)
				.Where(x => x.FinishDate.Year == year)
				.Where(x => x.AbonentId == abonentId);

			foreach(var credit in fullyPaidCredits)
			{
				credit.FullyPaid = false;
				credit.PaidForTheEntirePeriod -= credit.PaymentForMonth;
				credit.PaidMonths -= 1;
				abonentCreditRepository.Update(credit);
			}

			var activeCredits = abonentCreditRepository.Get()
				.Where(x => x.AbonentId == abonentId)
				.Where(x => x.FullyPaid == false);

			foreach (var credit in activeCredits)
			{
				credit.PaidForTheEntirePeriod -= credit.PaymentForMonth;
				credit.PaidMonths -= 1;
				abonentCreditRepository.Update(credit);
			}

			unitOfWork.Save();

			List<string> warnings = new List<string>();

			OpenMonthForAbonent(abonentRepository.GetById(abonentId), date, ref warnings);

			double balance = paymentRepository.Get()
										.Where(x => x.AbonentId == abonentId)
										.Where(x => x.Type == "Списание")
										.Where(x => x.Date.Month == date.Month)
										.Where(x => x.Date.Year == date.Year)
										.OrderBy(x => x.Date)
										.Last()
										.Balance;

			foreach (var payment in paymentRepository.Get()
										.Where(x => x.AbonentId == abonentId)
										.Where(x=>x.Type == "Пополнение")
										.Where(x=>x.Date.Month == date.Month)
										.Where(x=>x.Date.Year == date.Year)
										.OrderBy(x => x.Date))
			{
				payment.Balance = balance + payment.Sum;
				balance = payment.Balance;
				paymentRepository.Update(payment);
			}

			unitOfWork.Save();
				
			IQueryable<Payment> payments = paymentRepository.Get().Include(x => x.Abonent);

			payments = payments.Where(x => x.Date.Year == date.Year)
				.Where(x => x.Date.Month == date.Month)
				.Where(x => x.AbonentId == abonentId);

			return View("OpenMonthSummary", new OpenMonthSummaryViewModel(warnings, payments.ToList()));
		}

		public PartialViewResult All(string AbonentNumber, string Year, int? Month, string PaymentType)
		{
			IQueryable<Payment> payments = paymentRepository.Get().Include(x=>x.Abonent);

			if (!string.IsNullOrEmpty(AbonentNumber) && AbonentNumber != "Все")
				payments = payments.Where(x=>x.Abonent.AbonentNumber == AbonentNumber);
			if (!string.IsNullOrEmpty(Year) && Year != "Все")
				payments = payments.Where(x => x.Date.Year == TryToParseInt(Year));
			if (Month.HasValue && Month != 0)
				payments = payments.Where(x => x.Date.Month == Month.Value);
			if (!string.IsNullOrEmpty(PaymentType) && PaymentType!= "Все")
				payments = payments.Where(x => x.Type == PaymentType);

			return PartialView(payments.OrderBy(x => x.Date));
		}

		public PartialViewResult Filter(string AbonentNumber, string Year, int? Month, string PaymentType)
		{
			IQueryable<Payment> payments = paymentRepository.Get().Include(x => x.Abonent);

			if (!string.IsNullOrEmpty(AbonentNumber) && AbonentNumber != "Все")
				payments = payments.Where(x => x.Abonent.AbonentNumber == AbonentNumber);
			if (!string.IsNullOrEmpty(Year) && Year != "Все")
				payments = payments.Where(x => x.Date.Year == TryToParseInt(Year));
			if (Month.HasValue && Month != 0)
				payments = payments.Where(x => x.Date.Month == Month.Value);
			if (!string.IsNullOrEmpty(PaymentType) && PaymentType != "Все")
				payments = payments.Where(x => x.Type == PaymentType);

			return PartialView(new IndexViewModel(payments));
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

		public ActionResult Details(int id = 0)
		{
			Payment payment = paymentRepository.GetById(id);
			if (payment == null)
			{
				return HttpNotFound();
			}
			return View(payment);
		}

		private bool CheckOpenMonthAbility(DateTime date)
		{
			if (paymentRepository.Get().Where(x => x.Date.Month == date.Month)
									   .Where(x=>x.Date.Year == date.Year)
									   .Where(x => x.Type == "Списание")
									   .Any())
				return true;
			return false;
		}

		public struct PDFReceipt
		{
			public MemoryStream workStream { get; set; }
			public Document document { get; set; }
			public PdfPTable Table { get; set; }
			public iTextSharp.text.Font HeaderFont { get; set; }
			public iTextSharp.text.Font TextFont { get; set; }
		}
		public struct CounterProperties
		{
			public double CurrentValue { get; set; }
			public double PreviousValue { get; set; }
			public double CounterValueForPayment { get; set; }
			public double SummForPayment { get; set; }
		}

		private void GenerateCreditReceiptPart(ref PDFReceipt receipt, AbonentCredit abonentCredit)
		{
			PdfPCell cell = new PdfPCell(new Phrase(abonentCredit.Credit.Name, receipt.HeaderFont));
			cell.BorderWidth = 0.5f;
			cell.Colspan = 2;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(abonentCredit.PaidForTheEntirePeriod.ToString("F2"), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(abonentCredit.Price.ToString("F2"), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(abonentCredit.PaymentForMonth.ToString("F2"), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(abonentCredit.PaymentForMonth.ToString("F2"), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(abonentCredit.PaymentForMonth.ToString("F2"), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);
		}
		private void GenerateFirstTypeServiceReceiptPart(ref PDFReceipt receipt, AbonentService aService)
		{
			PdfPCell cell = new PdfPCell(new Phrase(aService.Service.Name, receipt.HeaderFont));
			cell.BorderWidth = 0.5f;
			cell.Colspan = 2;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(aService.Service.Price.ToString(), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(aService.Service.Price.ToString(), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(aService.Service.Price.ToString(), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(aService.Service.Price.ToString(), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);
		}
		private void GenerateSecondTypeServiceReceiptPart(ref PDFReceipt receipt, AbonentService aService, double aServiceSumm)
		{
			PdfPCell cell = new PdfPCell(new Phrase(aService.Service.Name, receipt.HeaderFont));
			cell.BorderWidth = 0.5f;
			cell.Colspan = 2;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(aService.Service.Price.ToString(), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(aServiceSumm.ToString(), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(aServiceSumm.ToString(), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(aServiceSumm.ToString(), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);
		}
		private void GenerateThirdTypeServiceReceiptPart(ref PDFReceipt receipt, AbonentService aService, CounterProperties counterProperties)
		{
			PdfPCell cell = new PdfPCell(new Phrase(aService.Service.Name, receipt.HeaderFont));
			cell.BorderWidth = 0.5f;
			cell.Colspan = 2;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(counterProperties.PreviousValue.ToString("F2"), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(counterProperties.CurrentValue.ToString("F2"), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase((counterProperties.CounterValueForPayment).ToString("F2"), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(aService.Service.Price.ToString(), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(counterProperties.SummForPayment.ToString("F2"), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(@"  "));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(counterProperties.SummForPayment.ToString("F2"), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(counterProperties.SummForPayment.ToString("F2"), receipt.TextFont));
			cell.BorderWidth = 0.5f;
			receipt.Table.AddCell(cell);
		}

		public void CalculateCreditPayment(Abonent abonent, AbonentCredit aCredit, DateTime date, ref PDFReceipt receipt)
		{
			//CheckPaymentAbility(abonent, date);
			Payment creditPayment = new Payment()
					{
						AbonentId = abonent.AbonentId,
						Balance = paymentRepository.Get().Where(x => x.AbonentId == abonent.AbonentId).Where(x=>x.Date < date).OrderBy(x=>x.Date).Last().Balance - aCredit.PaymentForMonth,
						Date = date,
						Sum = aCredit.PaymentForMonth,
						Comment = "Списание по кредиту " + aCredit.Credit.Name + " за " + (aCredit.PaidMonths + 1).ToString() + " месяц",
						Type = "Списание"
					};

			aCredit.PaidMonths += 1;
			aCredit.PaidForTheEntirePeriod += aCredit.PaymentForMonth > 0 ? aCredit.PaymentForMonth : 0;
			if (aCredit.PaidMonths == aCredit.Term)
				aCredit.FullyPaid = true;

			abonentCreditRepository.Update(aCredit);
			paymentRepository.Insert(creditPayment);

			GenerateCreditReceiptPart(ref receipt, aCredit);

			unitOfWork.Save();
		}
		public void CalculateFirstTypeServicePayment(Abonent abonent, AbonentService aService, DateTime date, ref PDFReceipt receipt)
		{
			//CheckPaymentAbility(abonent, date);

			Payment servicePayment = new Payment()
			{
				AbonentId = abonent.AbonentId,
				Balance = paymentRepository.Get().Where(x => x.AbonentId == abonent.AbonentId).Where(x => x.Date < date).OrderBy(x => x.Date).Last().Balance - aService.Service.Price,
				Date = date, 
				Sum = aService.Service.Price,
				Comment = "Списание по сервису " + aService.Service.Name + " за " + date.Month.ToString() + " месяц",
				Type = "Списание"
			};
			paymentRepository.Insert(servicePayment);

			GenerateFirstTypeServiceReceiptPart(ref receipt, aService);

			unitOfWork.Save();
		}
		public void CalculateSecondTypeServicePayment(Abonent abonent, AbonentService aService, DateTime date,  ref PDFReceipt receipt)
		{
			//CheckPaymentAbility(abonent, date);
			Payment servicePayment = new Payment()
			{
				AbonentId = abonent.AbonentId,
				Balance = paymentRepository.Get().Where(x => x.AbonentId == abonent.AbonentId).Where(x => x.Date <= date).OrderBy(x => x.Date).Last().Balance - aService.Service.Price * abonent.NumberOfInhabitants,
				Date = date,
				Sum = aService.Service.Price * abonent.NumberOfInhabitants,
				Comment = "Списание по сервису " + aService.Service.Name + " за " + date.Month.ToString() + " месяц",
				Type = "Списание"
			};
			paymentRepository.Insert(servicePayment);

			GenerateSecondTypeServiceReceiptPart(ref receipt, aService, aService.Service.Price * abonent.NumberOfInhabitants);

			unitOfWork.Save();
		}
		public List<string> CalculateThirdTypeServicePayment(Abonent abonent, AbonentService aService, DateTime date, List<string> warnings, ref PDFReceipt receipt)
		{
			//CheckPaymentAbility(abonent, date);

			IQueryable<Counter> counters = counterRepository.Get().Where(x => x.AbonentId == abonent.AbonentId)
													 .Where(x => x.ServiceId == aService.ServiceId)
													 .Where(x => x.Service.isOff == false)
													 .Include(x=>x.CounterValues);

			if (counters.Count() > 1)
				warnings.Add("У абонента" + abonent.AbonentNumber + " " + abonent.Name + " обнаружено " + counters.Count() + " счетчиков по услуге " + aService.Service.Name);

			if (counters.Count() == 0)
			{
				warnings.Add("У абонента" + abonent.AbonentNumber + " " + abonent.Name + " не обнаружено счетчиков по услуге " + aService.Service.Name);
				return warnings; 
			}

			Counter aCounter = counters.LastOrDefault();

			aCounter.CounterValues = counterValuesRepository.Get().Where(x => x.CounterId == aCounter.CounterId).ToList();
			
			CounterProperties counterProperties = new CounterProperties()
			{
				CurrentValue = aCounter.CounterValues.LastOrDefault().Value,
				PreviousValue = 0
			};

			if ((aCounter.CounterValues.Count >2 )&&(aCounter.CounterValues.Last().Date.Month != date.Month - 1 || aCounter.CounterValues.Last().Date.Month != date.Month))
			{
				warnings.Add("У абонента " + abonent.AbonentNumber + " " + abonent.Name + " не обнаружено данных по счетчику " + aCounter.Name + " по услуге " + aService.Service.Name);
			}
			else
			{
				if (aCounter.CounterValues.Count<2)
				{
					counterProperties.PreviousValue = aCounter.InitialValue;
				}
				else
				{
					counterProperties.PreviousValue = aCounter.CounterValues.ToList()[aCounter.CounterValues.Count - 2].Value;
				}
			}

			counterProperties.CounterValueForPayment = counterProperties.CurrentValue - counterProperties.PreviousValue;
			counterProperties.SummForPayment = 0;
			
			int minimalLimit = 0;
			if (aService.Service.Tariffs.Any())
			minimalLimit = aService.Service.Tariffs.Min(x=>x.LimitValue);

			if (aService.Service.Tariffs.Any() && (minimalLimit < counterProperties.CounterValueForPayment))
			{
				var tariffs = aService.Service.Tariffs.OrderByDescending(x => x.LimitValue);
				foreach (var tarif in tariffs)
				{
					if (counterProperties.CounterValueForPayment > tarif.LimitValue)
					{
						counterProperties.SummForPayment += (counterProperties.CounterValueForPayment - tarif.LimitValue) * tarif.OverPrice;
						counterProperties.CounterValueForPayment = tarif.LimitValue;
					}
				}
				counterProperties.SummForPayment += tariffs.Last().LimitValue * aService.Service.Price;
			}
			else
			{
				counterProperties.SummForPayment = counterProperties.CounterValueForPayment * aService.Service.Price;
			}     
		   
			Payment servicePayment = new Payment()
				{
					AbonentId = abonent.AbonentId,
					Balance = paymentRepository.Get().Where(x => x.AbonentId == abonent.AbonentId).Where(x => x.Date < date).OrderBy(x => x.Date).Last().Balance - counterProperties.SummForPayment,
					Date = date,
					Sum = counterProperties.SummForPayment,
					Comment = "Списание по сервису " + aService.Service.Name + " за " + date.Month.ToString() + " месяц",
					Type = "Списание"
				};
			paymentRepository.Insert(servicePayment);

			GenerateThirdTypeServiceReceiptPart(ref receipt, aService, counterProperties);

			unitOfWork.Save();
			return warnings;
		}

		public PDFReceipt PDFReceiptInitializer(Abonent abonent, DateTime date)
		{
			MemoryStream workStream = new MemoryStream();
			Document document = new Document();
			PdfWriter.GetInstance(document, workStream).CloseStream = false;

			String FONT_LOCATION = Server.MapPath("~/Content/fonts/ARIAL.ttf");
			BaseFont baseFont = BaseFont.CreateFont(FONT_LOCATION, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
			iTextSharp.text.Font headerFont = new iTextSharp.text.Font(baseFont, 8);
			iTextSharp.text.Font infoFont = new iTextSharp.text.Font(baseFont, 10); 

			document.Open();

			document.SetMargins(0.1f,0.1f,0.1f,0.1f);

			PdfPTable table = new PdfPTable(12);

			table.WidthPercentage = 100;
			table.SpacingBefore = 0.5f;
			table.SpacingAfter = 0.5f;

			PDFReceipt receipt = new PDFReceipt()
			{
				HeaderFont = headerFont,
				TextFont = infoFont,
				Table = table,
				document = document,
				workStream = workStream
			};

			#region PDF_HEADER
			PdfPCell cell = new PdfPCell(new Phrase(String.Concat("Шот", Environment.NewLine, "Счет"), headerFont));
			cell.BorderWidth = 0;
			cell.Colspan = 2;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(date.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU")) + " " + date.Year.ToString(), infoFont));
			cell.BorderWidth = 0;
			cell.Colspan = 2;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("Дебрес шос", Environment.NewLine, "Лицевой счет"), headerFont));
			cell.BorderWidth = 0;
			cell.Colspan = 2;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(abonent.AbonentNumber, infoFont));
			cell.BorderWidth = 0;
			cell.Colspan = 3;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(string.Concat("Адам саны", Environment.NewLine, "Кол. Чел"), headerFont));
			cell.BorderWidth = 0;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(abonent.NumberOfInhabitants.ToString(), infoFont));
			cell.BorderWidth = 0;
			cell.Colspan = 2;
			table.AddCell(cell);
			#endregion

			double balance = GetBalance(abonent,date);

			#region PDF_NAME_ROW

			cell = new PdfPCell(new Phrase(@"Телеушi/Плательщик", headerFont));
			cell.BorderWidth = 0;
			cell.Colspan = 2;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(abonent.Name, infoFont));
			cell.BorderWidth = 0;
			cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
			cell.Colspan = 7;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("Теңгерім", Environment.NewLine, "Баланс"), headerFont));
			cell.BorderWidth = 0;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(balance.ToString("F2"), infoFont));
			cell.BorderWidth = 0;
			cell.Colspan = 2;
			table.AddCell(cell);

			#endregion
			#region PDF_ADRESS_ROW

			cell = new PdfPCell(new Phrase(@"Мекен-жайы/Адрес", headerFont));
			cell.BorderWidth = 0;
			cell.Colspan = 2;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(unitOfWork.CityRepository.GetById(abonent.Address.CityId).Name 
				+ " " + unitOfWork.StreetRepository.GetById(abonent.Address.StreetId).Name
				+ " " + abonent.Address.House, infoFont));
			cell.BorderWidth = 0;
			cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
			cell.Colspan = 10;
			table.AddCell(cell);
			#endregion
			#region PDF_SECOND_HEADER
			cell = new PdfPCell(new Phrase(String.Concat("Қызметтердің аталуы", Environment.NewLine, "наименование услуги"), headerFont));
			cell.BorderWidth = 0.5f;
			cell.Colspan = 2;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("теленген", Environment.NewLine, "оплаченно"), headerFont));
			cell.BorderWidth = 0.5f;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("соңғы", Environment.NewLine, "предыдущее"), headerFont));
			cell.BorderWidth = 0.5f;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("алдынғы", Environment.NewLine, "последнее"), headerFont));
			cell.BorderWidth = 0.5f;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("мөлшері", Environment.NewLine, "количество"), headerFont));
			cell.BorderWidth = 0.5f;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("баға", Environment.NewLine, "цена"), headerFont));
			cell.BorderWidth = 0.5f;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("есеп. көрсет.", Environment.NewLine, "начисленые показания"), headerFont));
			cell.BorderWidth = 0.5f;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("өсімақысы", Environment.NewLine, "пеня"), headerFont));
			cell.BorderWidth = 0.5f;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("қарыз", Environment.NewLine, "переплата/долг"), headerFont));
			cell.BorderWidth = 0.5f;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("телемге", Environment.NewLine, "к оплате"), headerFont));
			cell.BorderWidth = 0.5f;
			table.AddCell(cell);

			cell = new PdfPCell(new Phrase(String.Concat("телеймін", Environment.NewLine, "оплачиваю"), headerFont));
			cell.BorderWidth = 0.5f;
			table.AddCell(cell);
			#endregion

			return receipt;
		}

		double GetBalance(Abonent abonent, DateTime date)
		{
			double balance = 0;
			if (abonent.Payments.Where(x => x.Date < date).Any() && abonent.Payments != null)
			{
				balance = abonent.Payments.Where(x => x.Date < date).OrderBy(x => x.Date).LastOrDefault().Balance;
			}
			return balance;
		}
		public PdfPTable PDFReceiptWhiteSpasecAdder(int count)
		{
			PdfPTable table = new PdfPTable(12);
			for (int i = 0; i < count; i++)
			{
				PdfPCell cell = new PdfPCell(new Phrase("  "));
				cell.Colspan = 12;
				cell.BorderWidth = 0;
				table.AddCell(cell);
			}

			return table;
		}
		public void PDFReceiptSummAdder(ref PDFReceipt receipt, double monthPaymentSumm, double monthAfterPaymentBalance)
		{	
			PdfPCell cell = new PdfPCell(new Phrase(@"  "));
			cell.Colspan = 9;
			cell.BorderWidth = 0;
			receipt.Table.AddCell(cell);
			
			cell = new PdfPCell(new Phrase(String.Concat("Барлығы", Environment.NewLine, "Итого"), receipt.HeaderFont));
			cell.BorderWidth = 1;
			receipt.Table.AddCell(cell);
			
			cell = new PdfPCell(new Phrase(monthPaymentSumm.ToString("F2"), receipt.HeaderFont));
			cell.BorderWidth = 1;
			receipt.Table.AddCell(cell);

			cell = new PdfPCell(new Phrase(monthAfterPaymentBalance.ToString("F2"), receipt.HeaderFont));
			cell.BorderWidth = 1;
			receipt.Table.AddCell(cell);
		}
		public void PDFReceiptFinalizer(ref PDFReceipt receipt, Abonent abonent, DateTime date)
		{
			receipt.document.Add(receipt.Table);
			receipt.document.Add(PDFReceiptWhiteSpasecAdder(10));
			receipt.document.Add(receipt.Table);
			receipt.document.Close();

			byte[] byteInfo = receipt.workStream.ToArray();
			receipt.workStream.Write(byteInfo, 0, byteInfo.Length);
			receipt.workStream.Position = 0;

			Domain.Entities.PDFStorages.PDFDocument PDFDoc = new Domain.Entities.PDFStorages.PDFDocument()
			{
				Date = date, 
				AbonentId = abonent.AbonentId,
				Value = byteInfo,
				TimeStamp = DateTime.Now
			};

			unitOfWork.PDFDocumentRepository.Insert(PDFDoc);
		}

		public void OpenMonthForAbonent(Abonent abonent, DateTime date, ref List<string> warnings)
		{
			PDFReceipt receipt = PDFReceiptInitializer(abonent, date);
			foreach (AbonentCredit aCredit in abonentCreditRepository.Get().Where(x => x.AbonentId == abonent.AbonentId)
																		  .Where(x => x.FullyPaid != true)
																		  .Include(x => x.Credit))
			{
				CalculateCreditPayment(abonent, aCredit, date, ref receipt);
			}

			var aServices = abonentServiceRepository.Get().Where(x => x.AbonentId == abonent.AbonentId)
																			  .Where(x=> x.isOff == false || (x.isOff== true && x.FinishDate > date ))
																			  .Where(x => x.StartDate <= date.AddMonths(1))
																			  .Include(x => x.Service);

			foreach (AbonentService aService in aServices)
			{
				switch (aService.Service.Type)
				{
					case 1:
						{
							CalculateFirstTypeServicePayment(abonent, aService, date, ref receipt);
							break;
						}
					case 2:
						{
							CalculateSecondTypeServicePayment(abonent, aService, date, ref receipt);
							break;
						}
					case 3:
						{
							warnings = CalculateThirdTypeServicePayment(abonent, aService, date, warnings, ref receipt);
							break;
						}
				}
			}

			double monthPaymentSumm = paymentRepository.Get()
				.Where(x => x.Type == "Списание")
				.Where(x => x.Date.Month == date.Month)
				.Where(x => x.Date.Year == date.Year)
				.Where(x=>x.AbonentId == abonent.AbonentId)
				.Select(x => x.Sum)
				.Sum();
			
			double balance = GetBalance(abonent, date);

			double monthAfterPaymentBalance = monthPaymentSumm - balance;

			PDFReceiptSummAdder(ref receipt, monthPaymentSumm, monthAfterPaymentBalance);
			PDFReceiptFinalizer(ref receipt, abonent, date);
			unitOfWork.Save();                       
		}

		public ActionResult OpenMonth()
		{
			if (abonentRepository.Get()
				.Where(x=>x.isDeleted == false)
				.Where(x=>(string.IsNullOrWhiteSpace(x.AbonentNumber)))
				.Any())
			{
				ModelState.AddModelError(string.Empty, "Существуют абоненты без номера. Открытие месяца невозможно");
				ViewBag.HasErrors = true;
				return View();
			}
			var abonentsThatHasPayment = paymentRepository.Get()
											.Select(x => x.Abonent.AbonentNumber)
											.Distinct();
			var allAbonents = unitOfWork.AbonentRepository.Get();
			var abonentsWithOutPayment = from abonents in allAbonents 
										 where (!abonentsThatHasPayment.Contains(abonents.AbonentNumber))
										 select abonents.AbonentNumber;
			//if (!string.IsNullOrEmpty(abonentsWithOutPayment.First()))
			if ((!String.IsNullOrEmpty(abonentsWithOutPayment.FirstOrDefault())&& abonentsWithOutPayment.Any()))
			{
				string error = "Данные пользователи ";
				foreach (var abonent in abonentsWithOutPayment)
				{
					error += abonent + ", ";
				}
				error = error.Insert(error.Length - 2, " не имеют платежей в базе. Для открытия месяца у каждого пользователя должен быть хотя бы один платеж");
				ModelState.AddModelError(string.Empty, error);
				ViewBag.HasErrors = true;
			}
			else
			{
				ViewBag.HasErrors = false;
			}
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult OpenMonth(DateTime date)
		{
			//if (CheckOpenMonthAbility(date))
			//{
			//    ModelState.AddModelError("date", "Этот месяц уже открыт");
			//    return View();
			//}

			List<string> warnings = new List<string>();

			foreach (Abonent abonent in abonentRepository.Get().Where(x => x.isDeleted != true))
			{
				OpenMonthForAbonent(abonent, date, ref warnings);                                                                         
			}

			IQueryable<Payment> payments = paymentRepository.Get().Include(x => x.Abonent);

			payments = payments.Where(x => x.Date.Year == date.Year);
			payments = payments.Where(x => x.Date.Month == date.Month);

			return View("OpenMonthSummary", new OpenMonthSummaryViewModel(warnings, payments.ToList()));
		}

		public PartialViewResult CheckMonthOpenAbility(DateTime? date)
		{
			Dictionary<string, int> result = new Dictionary<string, int>();

			foreach (var item in  unitOfWork.ServiceRepository.Get().Where(x=>x.Type == 3))
			{
				List<CounterValues> counterValuesNumber = counterValuesRepository.Get().Where(x => x.Date.Month == date.Value.Month)
												 .Where(x => x.Counter.ServiceId == item.ServiceId)
												 .ToList();
				if (counterValuesNumber.Any())
				{
					result.Add(item.Name, counterValuesNumber.Count);
				}
				else
				{
					result.Add(item.Name, 0);
				}
			}
			return PartialView(result);        
		}

		public ActionResult Create()
		{
			ViewBag.PaymentTypes = new SelectList(paymentRepository.Get().Select(x => x.Type).Distinct());
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
				payment.AbonentId = abonentRepository.Get().Where(x => x.AbonentNumber == AbonentNumber)
														   .Where(x => x.isDeleted == false)
														   .Select(x => x.AbonentId)
														   .LastOrDefault();

				double balance  = paymentRepository.Get().Where(x=>x.AbonentId == payment.AbonentId)
													  .Select(x=>x.Balance)
													  .LastOrDefault();
				if (payment.Type == "Списание")
					balance -= payment.Sum;
				else
					balance += payment.Sum;
				payment.Balance = balance;
			}
			if (ModelState.IsValid)
			{
				paymentRepository.Insert(payment);
				unitOfWork.Save();
				return RedirectToAction("Index");
			}
			ViewBag.PaymentTypes = new SelectList(paymentRepository.Get().Select(x => x.Type).Distinct());
			return View(payment);
		}
		public FileStreamResult GetPDF(int abonentId, int year, string month)
		{
			Domain.Entities.PDFStorages.PDFDocument doc = unitOfWork.PDFDocumentRepository.Get()
				.Where(x => x.AbonentId == abonentId)
				.Where(x => x.Date.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU")) == month)
				.Where(x => x.Date.Year == year)
				.LastOrDefault();

			MemoryStream workStream = new MemoryStream(doc.Value);
			workStream.Position = 0;

			return new FileStreamResult(workStream, "application/pdf");
		}

		public ActionResult Numerous()
		{
			var abonents = abonentRepository.Get().Where(x => x.isDeleted == false)
												  .Include(x => x.Payments)
												  .ToList();

			ViewBag.Abonents = abonents;
			List<NumerousViewModel> abonentModels = new List<NumerousViewModel>();

			foreach (var item in abonents)
			{ 
				abonentModels.Add(new NumerousViewModel(item.AbonentNumber, item.Name, item.AbonentId, item.Payments.Any()? item.Payments.LastOrDefault().Balance : 0));
			}

			return View(abonentModels);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Numerous(List<NumerousViewModel> Numerous, DateTime date)
		{
			foreach (var item in Numerous)
			{
				if (item.Sum != 0)
				{
					Payment payment = new Payment()
					{
						AbonentId = item.AbonentId,
						Balance = item.Balance += item.Sum,
						Comment = "Пополнение счета за " + date.ToString("D", CultureInfo.CreateSpecificCulture("ru-RU")),
						Type = "Пополнение",
						Date = date,
						Sum = item.Sum
					};
					paymentRepository.Insert(payment);
				}
			}
			unitOfWork.Save();
			return RedirectToAction("Index");
		}

		public PartialViewResult RecalculateBalance(int? abonentId)
		{
			double balance = 0;
			foreach (var payment in paymentRepository.Get().Where(x=>x.AbonentId == abonentId).OrderBy(x=>x.Date))
			{
				if (payment.Type == "Списание")
				{
					payment.Balance = balance - payment.Sum;
					balance = payment.Balance;
				}
				else
				{
					payment.Balance = balance + payment.Sum;
					balance = payment.Balance;
				}
				paymentRepository.Update(payment);
			}
			unitOfWork.Save();
			return PartialView(balance);
		}
		////
		//// POST: /Payment/Delete/5
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
	}
}