using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Counters;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Payments;
using DalaWeb.Domain.Entities.Services;
using DalaWeb.Domain.Entities.Settings;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.Infrastructure
{
    public struct PDFReceipt
    {
        public MemoryStream workStream;
        public Document document;
        public PdfPTable table;
        public iTextSharp.text.Font headerFont;
        public iTextSharp.text.Font textFont;

        public PDFReceipt(int headerFontSize, int infoFontSize, string FontLocation)
        {
            workStream = new MemoryStream();
            this.document = new Document();
            PdfWriter.GetInstance(document, workStream).CloseStream = false;

            BaseFont baseFont = BaseFont.CreateFont(FontLocation, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            headerFont = new iTextSharp.text.Font(baseFont, headerFontSize);
            textFont = new iTextSharp.text.Font(baseFont, infoFontSize);

            document.Open();
            document.SetMargins(0.1f, 0.1f, 0.1f, 0.1f);

            table = new PdfPTable(12);

            table.WidthPercentage = 100;
            table.SpacingBefore = 0.5f;
            table.SpacingAfter = 0.5f;
        }
    }
    public sealed class PDFAbonentMonthlyReceiptGenerator
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Payment> paymentRepository;
        private IRepository<Abonent> abonentRepository;
        private IRepository<AbonentCredit> abonentCreditRepository;
        private IRepository<AbonentService> abonentServiceRepository;
        private IRepository<Counter> counterRepository;
        private IRepository<CounterValues> counterValuesRepository;
        private IRepository<Service> serviceRepository;
        private IRepository<Setting> settingRepository;

        private DateTime date;
        private Abonent abonent;
        private PDFReceipt receipt;
        private string FontLocation;

        public PDFAbonentMonthlyReceiptGenerator(IUnitOfWork unitOfWork, Abonent abonent, DateTime date, string FontLocation)
        {
            this.unitOfWork = unitOfWork;
            this.abonent = abonent;
            this.date = date;
            this.FontLocation = FontLocation;
            paymentRepository = unitOfWork.PaymentRepository;
            abonentRepository = unitOfWork.AbonentRepository;
            abonentCreditRepository = unitOfWork.AbonentCreditRepository;
            abonentServiceRepository = unitOfWork.AbonentServiceRepository;
            counterRepository = unitOfWork.CounterRepository;
            counterValuesRepository = unitOfWork.CounterValuesRepository;
            serviceRepository = unitOfWork.ServiceRepository;
            settingRepository = unitOfWork.SettingRepository;
        }

        public void GenerateAbonentPDFReceipt()
        {
            receipt = new PDFReceipt(10, 12, FontLocation);

            GenerateReceiptHeader();

            GenerateNameRow(paymentRepository.Get()
                .Where(x => x.Abonent.AbonentId == abonent.AbonentId)
                .Where(x => x.Date < date)
                .Sum(x => x.Sum));

            string address = abonent.Address.City.Name
                + " " + abonent.Address.Street.Name
                + " " + abonent.Address.House;

            GenerateAddressRow(address);
            GenerateTableHeaders();
            GeneratePayments();

            double monthPaymentSum = paymentRepository.Get()
                .Where(x=>x.Abonent.AbonentId == abonent.AbonentId)
                .Where(x => x.Date == date)
                .Select(x => x.Sum)
                .Sum();
            double afterMonthPaymentBalance = paymentRepository.Get()
                .Where(x=>x.Abonent.AbonentId == abonent.AbonentId)
                .Where(x=>x.Date <= date)
                .Select(x => x.Sum)
                .Sum();

            GenerateSumm(monthPaymentSum, afterMonthPaymentBalance);

            SaveAbonentPDFReceipt();
        }
        private void GenerateReceiptHeader()
        {
            PdfPCell cell = new PdfPCell(new Phrase(String.Concat("Шот", Environment.NewLine, "Счет"), receipt.headerFont));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(date.ToString("MMMM")+ " " + date.Year.ToString(), receipt.textFont));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("Дебрес шос", Environment.NewLine, "Лицевой счет"), receipt.headerFont));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonent.AbonentNumber, receipt.textFont));
            cell.BorderWidth = 0;
            cell.Colspan = 3;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(string.Concat("Адам саны", Environment.NewLine, "Кол. Чел"), receipt.headerFont));
            cell.BorderWidth = 0;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonent.NumberOfInhabitants.ToString(), receipt.textFont));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);
        }
        private void GenerateNameRow(double balance)
        {
            PdfPCell cell = new PdfPCell(new Phrase(@"Телеушi/Плательщик", receipt.headerFont));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonent.Name, receipt.textFont));
            cell.BorderWidth = 0;
            cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            cell.Colspan = 7;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("Теңгерім", Environment.NewLine, "Баланс"), receipt.headerFont));
            cell.BorderWidth = 0;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(balance.ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);
        }
        private void GenerateAddressRow(string address)
        {
            PdfPCell cell = new PdfPCell(new Phrase(@"Мекен-жайы/Адрес", receipt.headerFont));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(address, receipt.textFont));
            cell.BorderWidth = 0;
            cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            cell.Colspan = 10;
            receipt.table.AddCell(cell);
        }
        private void GenerateTableHeaders()
        {
            PdfPCell cell = new PdfPCell(new Phrase(String.Concat("Қызметтердің аталуы", Environment.NewLine, "наименование услуги"), receipt.headerFont));
            cell.BorderWidth = 0.5f;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("теленген", Environment.NewLine, "оплаченно"), receipt.headerFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("соңғы", Environment.NewLine, "предыдущее"), receipt.headerFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("алдынғы", Environment.NewLine, "последнее"), receipt.headerFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("мөлшері", Environment.NewLine, "количество"), receipt.headerFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("баға", Environment.NewLine, "цена"), receipt.headerFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("есеп. көрсет.", Environment.NewLine, "начисленые показания"), receipt.headerFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("өсімақысы", Environment.NewLine, "пеня"), receipt.headerFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("қарыз", Environment.NewLine, "переплата/долг"), receipt.headerFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("телемге", Environment.NewLine, "к оплате"), receipt.headerFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("телеймін", Environment.NewLine, "оплачиваю"), receipt.headerFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);
        }
        private void GeneratePayments()
        {
            foreach (var payment in paymentRepository.Get()
                .Where(x => x.Abonent.AbonentId == abonent.AbonentId)
                .Where(x => x.Date == date))
            {
                if (payment.AbonentCredit != null)
                {
                    GenerateCreditReceipt(payment.AbonentCredit);
                }
                if (payment.AbonentService != null)
                {
                    AbonentService aService = payment.AbonentService;

                    switch (serviceRepository.GetById(aService.Service.ServiceId).Type)
                    {
                        case 1:
                            {
                                GenerateFirstTypeService(aService, payment.Sum);
                                break;
                            }
                        case 2:
                            {
                                GenerateSecondTypeService(aService, payment.Sum);
                                break;
                            }
                        case 3:
                            {
                                try
                                {
                                    CounterProperties counterProperties = GetCounterProperties(aService, payment);
                                    GenerateThirdTypeService(aService, counterProperties);
                                }
                                catch(Exception)
                                {
                                    // add error processing
                                }
                                break;
                            }
                    }
                }
                else
                {
                    //GenerateCustomPayment(payment);
                }
            }
        }
        private CounterProperties GetCounterProperties(AbonentService aService, Payment payment)
        {
            IQueryable<Counter> counters = counterRepository.Get().Where(x => x.Abonent.AbonentId == abonent.AbonentId)
                                             .Where(x => x.Service.ServiceId == aService.Service.ServiceId)
                                             .Where(x => ((x.isOff != true && x.StartDate <= date ) || (x.isOff == true && x.FinishDate > date)));

            CounterProperties counterProperties = new CounterProperties();

            if (counters.Count() == 0 || counters.Count() > 1)
            {
                throw new Exception("У абонента" + abonent.AbonentNumber + " " + abonent.Name 
                    + " обнаружено " + counters.Count() + " активных счетчиков по услуге " 
                    + aService.Service.Name); 
            }

            Counter aCounter = counters.Single();

            //aCounter.CounterValues = counterValuesRepository.Get().Where(x => x.Counter == aCounter).Where(x => x.Date < date).ToList();

            counterProperties.CurrentValue = aCounter.CounterValues.Where(x => x.Date <= date).OrderBy(x => x.Date).LastOrDefault().Value;
            counterProperties.PreviousValue = aCounter.CounterValues.ToList()[aCounter.CounterValues.Count() - 2].Value;
            counterProperties.CounterValueForPayment = counterProperties.CurrentValue - counterProperties.PreviousValue;
            counterProperties.SummForPayment = 0;

            int minimalLimit = 0;
            if (aService.Service.Tariffs.Any())
                minimalLimit = aService.Service.Tariffs.Min(x => x.LimitValue);

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
                counterProperties.SummForPayment += tariffs.Last().LimitValue * aService.Service.ServicePrice.Where(x => x.StartDate < date).OrderBy(x => x.StartDate).Last().Price;
            }
            else
            {
                counterProperties.SummForPayment = counterProperties.CounterValueForPayment * aService.Service.ServicePrice.Where(x => x.StartDate < date).OrderBy(x => x.StartDate).Last().Price;
            }

            return counterProperties;
        }
        private void GenerateCreditReceipt(AbonentCredit abonentCredit)
        {
            PdfPCell cell = new PdfPCell(new Phrase(abonentCredit.Credit.Name, receipt.headerFont));
            cell.BorderWidth = 0.5f;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonentCredit.PaidForTheEntirePeriod.ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonentCredit.Price.ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonentCredit.PaymentForMonth.ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonentCredit.PaymentForMonth.ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonentCredit.PaymentForMonth.ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);
        }
        private void GenerateFirstTypeService(AbonentService aService, double aServiceSumm )
        {
            PdfPCell cell = new PdfPCell(new Phrase(aService.Service.Name, receipt.headerFont));
            cell.BorderWidth = 0.5f;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - aServiceSumm).ToString(), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - aServiceSumm).ToString(), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - aServiceSumm).ToString(), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - aServiceSumm).ToString(), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);
        }
        private void GenerateSecondTypeService(AbonentService aService, double aServiceSumm)
        {
            PdfPCell cell = new PdfPCell(new Phrase(aService.Service.Name, receipt.headerFont));
            cell.BorderWidth = 0.5f;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(aService.Service.ServicePrice.Where(x=>x.StartDate < date).OrderBy(x => x.StartDate).Last().Price.ToString(), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - aServiceSumm).ToString(), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - aServiceSumm).ToString(), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - aServiceSumm).ToString(), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);
        }
        private void GenerateThirdTypeService(AbonentService aService, CounterProperties counterProperties)
        {
            PdfPCell cell = new PdfPCell(new Phrase(aService.Service.Name, receipt.headerFont));
            cell.BorderWidth = 0.5f;
            cell.Colspan = 2;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(counterProperties.PreviousValue.ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(counterProperties.CurrentValue.ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((counterProperties.CounterValueForPayment).ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(aService.Service.ServicePrice.Where(x=>x.StartDate < date).OrderBy(x=>x.StartDate).Last().Price.ToString(), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - counterProperties.SummForPayment).ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(@"  "));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - counterProperties.SummForPayment).ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - counterProperties.SummForPayment).ToString("F2"), receipt.textFont));
            cell.BorderWidth = 0.5f;
            receipt.table.AddCell(cell);
        }
        private void GenerateCustomPayment(Payment payment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(payment.Comment, receipt.headerFont));
            cell.Colspan = 9;
            cell.BorderWidth = 0;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("Барлығы", Environment.NewLine, "Итого"), receipt.headerFont));
            cell.BorderWidth = 1;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - payment.Sum).ToString("F2"), receipt.headerFont));
            cell.BorderWidth = 1;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - payment.Sum).ToString("F2"), receipt.headerFont));
            cell.BorderWidth = 1;
            receipt.table.AddCell(cell);
        }
        private void GenerateSumm(double monthPaymentSumm, double monthAfterPaymentBalance)
        {
            PdfPCell cell = new PdfPCell(new Phrase(@"  "));
            cell.Colspan = 9;
            cell.BorderWidth = 0;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("Барлығы", Environment.NewLine, "Итого"), receipt.headerFont));
            cell.BorderWidth = 1;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - monthPaymentSumm).ToString("F2"), receipt.headerFont));
            cell.BorderWidth = 1;
            receipt.table.AddCell(cell);

            cell = new PdfPCell(new Phrase((0 - monthAfterPaymentBalance).ToString("F2"), receipt.headerFont));
            cell.BorderWidth = 1;
            receipt.table.AddCell(cell);
        }

        private void GenerateAfterReceiptSignatureText()
        {
            string sigantureText = settingRepository.Get().Single().SignatureText;
            PdfPCell cell = new PdfPCell(new Phrase(sigantureText, receipt.headerFont));
            cell.Colspan = 12;
            cell.BorderWidth = 0;
            receipt.table.AddCell(cell);
        }
        private PdfPTable PDFReceiptWhiteSpasecAdder(int count)
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
        private void SaveAbonentPDFReceipt()
        {
            receipt.document.Add(receipt.table);
            receipt.document.Add(PDFReceiptWhiteSpasecAdder(10));
            GenerateAfterReceiptSignatureText();
            receipt.document.Add(receipt.table);
            receipt.document.Close();

            byte[] byteInfo = receipt.workStream.ToArray();
            receipt.workStream.Write(byteInfo, 0, byteInfo.Length);
            receipt.workStream.Position = 0;

            Domain.Entities.PDFStorages.PDFAbonentMonthlyReceipt PDFDoc = new Domain.Entities.PDFStorages.PDFAbonentMonthlyReceipt()
            {
                Date = date,
                Abonent = abonent,
                Value = byteInfo,
                TimeStamp = DateTime.Now
            };

            unitOfWork.PDFAbonentMonthlyReceiptRepository.Insert(PDFDoc);
            unitOfWork.Save();
        }
    }
}