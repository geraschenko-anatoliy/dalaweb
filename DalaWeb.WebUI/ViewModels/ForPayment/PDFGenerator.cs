using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Payments;
using DalaWeb.Domain.Entities.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DalaWeb.WebUI.ViewModels.ForPayment
{
    public static class PDFGenerator
    {

        public static FileStreamResult Generate(Abonent abonent, BaseFont baseFont, IQueryable<Payment> monthPayments)
        {
            MemoryStream workStream = new MemoryStream();
            Document document = new Document();
            PdfWriter.GetInstance(document, workStream).CloseStream = false;

            iTextSharp.text.Font InfoFont = new iTextSharp.text.Font(baseFont, 8);
            iTextSharp.text.Font Font = InfoFont; //new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.BOLD, iTextSharp.text.Font.NORMAL);
            
            //iTextSharp.text.Font italicFont = new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.ITALIC);


            document.Open();

            PdfPTable table = new PdfPTable(12);

            #region TABLE_Properties
            table.WidthPercentage = 100;
            table.SpacingBefore = 0.5f;
            table.SpacingAfter = 0.5f;
            #endregion

            #region PDF_HEADER
            PdfPCell cell = new PdfPCell(new Phrase(String.Concat("Шот", Environment.NewLine, "Счет"), Font));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(DateTime.Now.ToShortDateString(), InfoFont));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("Дебрес шос", Environment.NewLine, "Лицевой счет"), InfoFont));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonent.AbonentNumber, InfoFont));
            cell.BorderWidth = 0;
            cell.Colspan = 3;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(string.Concat("Адам саны", Environment.NewLine, "Кол. Чел"), Font));
            cell.BorderWidth = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonent.NumberOfInhabitants.ToString(), InfoFont));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            table.AddCell(cell);
            #endregion
            #region PDF_NAME_ROW

            cell = new PdfPCell(new Phrase(@"Телеушi/Плательщик", Font));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            table.AddCell(cell);

            //string tempFIO = _DataView.Row["ФИО, Телефон"].ToString();
            //tempFIO = tempFIO.Substring(0, tempFIO.LastIndexOf(',') - 1);

            cell = new PdfPCell(new Phrase(abonent.Name, InfoFont));
            cell.BorderWidth = 0;
            cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            cell.Colspan = 7;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("Теңгерім", Environment.NewLine, "Баланс"), Font));
            cell.BorderWidth = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonent.Payments.LastOrDefault().Balance.ToString("F2"), InfoFont));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            table.AddCell(cell);

            #endregion
            #region PDF_ADRESS_ROW

            cell = new PdfPCell(new Phrase(@"Мекен-жайы/Адрес", Font));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(abonent.Address.CityId + " " + abonent.Address.StreetId, InfoFont));
            cell.BorderWidth = 0;
            cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            cell.Colspan = 10;
            table.AddCell(cell);
            #endregion
            #region PDF_SECOND_HEADER
            cell = new PdfPCell(new Phrase(String.Concat("Қызметтердің аталуы", Environment.NewLine, "наименование услуги"), Font));
            cell.BorderWidth = 0.5f;
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("теленген", Environment.NewLine, "оплаченно"), Font));
            cell.BorderWidth = 0.5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("соңғы", Environment.NewLine, "предыдущее"), Font));
            cell.BorderWidth = 0.5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("алдынғы", Environment.NewLine, "последнее"), Font));
            cell.BorderWidth = 0.5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("мөлшері", Environment.NewLine, "количество"), Font));
            cell.BorderWidth = 0.5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("баға", Environment.NewLine, "цена"), Font));
            cell.BorderWidth = 0.5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("есеп. көрсет.", Environment.NewLine, "начисленые показания"), Font));
            cell.BorderWidth = 0.5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("өсімақысы", Environment.NewLine, "пеня"), Font));
            cell.BorderWidth = 0.5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("қарыз", Environment.NewLine, "переплата/долг"), Font));
            cell.BorderWidth = 0.5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("телемге", Environment.NewLine, "к оплате"), Font));
            cell.BorderWidth = 0.5f;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(String.Concat("телеймін", Environment.NewLine, "оплачиваю"), Font));
            cell.BorderWidth = 0.5f;
            table.AddCell(cell);
            #endregion

            //#region PDF_BODY_SERVICE_COUNTERS

            ////var monthPayments = paymentRepository.Get().Where(x => x.AbonentId == abonentId)
            ////                                .Where(x => x.Date.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU")) == month)
            ////                                .Where(x => x.Date.Year == year);

            

            //foreach (var payment in monthPayments.Where(x=>x.ServiceId != null))
            //{

            //    AbonentService abonentService;
            //    if (payment.ServiceId != null)
            //        abonentService = abonent.AbonentServices.Where(x => x.ServiceId == payment.ServiceId)
            //                                                              .Where(x => x.isOff == false)
            //                                                              .Where(x=>x.Service.Type == 3)
            //                                                              .FirstOrDefault();

            //    //if (payment.CreditId == null)
            //    //    abonentCredit = abonent.AbonentCredits.Where(x => x.AbonentId == payment.AbonentId)
            //    //                                          .Where(x => x.CreditId == payment.CreditId)
            //    //                                          .FirstOrDefault();

            //    cell = new PdfPCell(new Phrase(abonentService.Service.Name, InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    cell.Colspan = 2;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);



            //    cell = new PdfPCell(new Phrase(_DataView.Row["Предыдущие показания"].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);
            //}







            //cell = new PdfPCell(new Phrase(_DataView.Row["Нынешние показания"].ToString(), InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["Кол-во киловатт"].ToString(), InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(ElectroServicePrice, InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["Сумма киловатт"].ToString(), InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(@"  ", InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(@"  ", InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["Сумма киловатт"].ToString(), InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["Сумма киловатт"].ToString(), InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);
            //#endregion


            //#region PDF_BODY_SERVICE_0
            //for (int i = 0; i < dt3.Rows.Count; i++)
            //{
            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[0].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    cell.Colspan = 2;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[3].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[2].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);
            //}
            //#endregion
            //#region PDF_BODY_SERVICE_1
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    cell = new PdfPCell(new Phrase(dt.Rows[i].ItemArray[0].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    cell.Colspan = 2;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //}
            //#endregion
            //#region PDF_BODY_SERVICE_2
            //for (int i = 0; i < dt2.Rows.Count; i++)
            //{
            //    cell = new PdfPCell(new Phrase(dt2.Rows[i].ItemArray[0].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    cell.Colspan = 2;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt2.Rows[i].ItemArray[2].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt2.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt2.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt2.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);
            //}
            //#endregion
            //#region PDF_BODY_SUMM
            //cell = new PdfPCell(new Phrase(@"  ", Font));
            //cell.Colspan = 9;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("Барлығы", Environment.NewLine, "Итого"), Font));
            //cell.BorderWidth = 1;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["ИТОГО"].ToString(), InfoFont));
            //cell.BorderWidth = 1;
            //table.AddCell(cell);

            //double allsumm = Convert.ToDouble(_DataView.Row["ИТОГО"].ToString().Replace('.', ',')) - Convert.ToDouble(_DataView.Row["БАЛАНС"].ToString().Replace('.', ','));

            //cell = new PdfPCell(new Phrase(allsumm.ToString().Replace(',', '.'), InfoFont));
            //cell.BorderWidth = 1;
            //table.AddCell(cell);
            //#endregion

            //#region PDF_SPACES_BETWEEN_COUNTERS
            //cell = new PdfPCell(new Phrase("  ", Font));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("  ", Font));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("  ", Font));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("  ", Font));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("  ", Font));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("  ", Font));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("  ", Font));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("  ", Font));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //cell.BorderWidthTop = 2;
            //table.AddCell(cell);
            //#endregion
            //#region PDF_HEADER
            //cell = new PdfPCell(new Phrase(String.Concat("Шот", Environment.NewLine, "Счет"), Font));
            //cell.BorderWidth = 0;
            //cell.Colspan = 2;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(GetDateToTable(), InfoFont));
            //cell.BorderWidth = 0;
            //cell.Colspan = 2;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("Дебрес шос", Environment.NewLine, "Лицевой счет"), Font));
            //cell.BorderWidth = 0;
            //cell.Colspan = 2;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["Лицевой счет"].ToString(), InfoFont));
            //cell.BorderWidth = 0;
            //cell.Colspan = 3;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(string.Concat("Адам саны", Environment.NewLine, "Кол. Чел"), Font));
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(NumberOfPersons.ToString(), InfoFont));
            //cell.BorderWidth = 0;
            //cell.Colspan = 2;
            //table.AddCell(cell);
            //#endregion
            //#region PDF_NAME_ROW

            //cell = new PdfPCell(new Phrase(@"Телеушi/Плательщик", Font));
            //cell.BorderWidth = 0;
            //cell.Colspan = 2;
            //table.AddCell(cell);

            ////string tempFIO = _DataView.Row["ФИО, Телефон"].ToString();
            ////tempFIO = tempFIO.Substring(0, tempFIO.LastIndexOf(',') - 1);
            //cell = new PdfPCell(new Phrase(tempFIO, InfoFont));
            //cell.BorderWidth = 0;
            //cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            //cell.Colspan = 7;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("Теңгерім", Environment.NewLine, "Баланс"), Font));
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["БАЛАНС"].ToString(), InfoFont));
            //cell.BorderWidth = 0;
            //cell.Colspan = 2;
            //table.AddCell(cell);

            //#endregion
            //#region PDF_ADRESS_ROW

            //cell = new PdfPCell(new Phrase(@"Мекен-жайы/Адрес", Font));
            //cell.BorderWidth = 0;
            //cell.Colspan = 2;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(FullAdress, InfoFont));
            //cell.BorderWidth = 0;
            //cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            //cell.Colspan = 10;
            //table.AddCell(cell);
            //#endregion
            //#region PDF_SECOND_HEADER
            //cell = new PdfPCell(new Phrase(String.Concat("Қызметтердің аталуы", Environment.NewLine, "наименование услуги"), Font));
            //cell.BorderWidth = 0.5f;
            //cell.Colspan = 2;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("теленген", Environment.NewLine, "оплаченно"), Font));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("соңғы", Environment.NewLine, "предыдущее"), Font));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("алдынғы", Environment.NewLine, "последнее"), Font));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("мөлшері", Environment.NewLine, "количество"), Font));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("баға", Environment.NewLine, "цена"), Font));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("есеп. көрсет.", Environment.NewLine, "начисленые показания"), Font));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("өсімақысы", Environment.NewLine, "пеня"), Font));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("қарыз", Environment.NewLine, "переплата/долг"), Font));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("телемге", Environment.NewLine, "к оплате"), Font));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("телеймін", Environment.NewLine, "оплачиваю"), Font));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);
            //#endregion
            //#region PDF_BODY_SERVICE_ELECTRO
            //cell = new PdfPCell(new Phrase("Электроэнергия", InfoFont));
            //cell.BorderWidth = 0.5f;
            //cell.Colspan = 2;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(@"  ", Font));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["Предыдущие показания"].ToString(), InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["Нынешние показания"].ToString(), InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["Кол-во киловатт"].ToString(), InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(ElectroServicePrice, InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["Сумма киловатт"].ToString(), InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(@"  ", InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(@"  ", InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["Сумма киловатт"].ToString(), InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["Сумма киловатт"].ToString(), InfoFont));
            //cell.BorderWidth = 0.5f;
            //table.AddCell(cell);
            //#endregion
            //#region PDF_BODY_SERVICE_0
            //for (int i = 0; i < dt3.Rows.Count; i++)
            //{
            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[0].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    cell.Colspan = 2;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[3].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[2].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt3.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);
            //}
            //#endregion
            //#region PDF_BODY_SERVICE_1
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    cell = new PdfPCell(new Phrase(dt.Rows[i].ItemArray[0].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    cell.Colspan = 2;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //}
            //#endregion
            //#region PDF_BODY_SERVICE_2
            //for (int i = 0; i < dt2.Rows.Count; i++)
            //{
            //    cell = new PdfPCell(new Phrase(dt2.Rows[i].ItemArray[0].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    cell.Colspan = 2;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt2.Rows[i].ItemArray[2].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt2.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(@"  ", Font));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt2.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);

            //    cell = new PdfPCell(new Phrase(dt2.Rows[i].ItemArray[1].ToString(), InfoFont));
            //    cell.BorderWidth = 0.5f;
            //    table.AddCell(cell);
            //}
            //#endregion
            //#region PDF_BODY_SUMM
            //cell = new PdfPCell(new Phrase(@"  ", Font));
            //cell.Colspan = 9;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(String.Concat("Барлығы", Environment.NewLine, "Итого"), Font));
            //cell.BorderWidth = 1;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(_DataView.Row["ИТОГО"].ToString(), InfoFont));
            //cell.BorderWidth = 1;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(allsumm.ToString().Replace(',', '.'), InfoFont));
            //cell.BorderWidth = 1;
            //table.AddCell(cell);
            //#endregion


            //#region PDF_FOOTER_TEXT
            //cell = new PdfPCell(new Phrase(@"  ", InfoFont));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("  ", Font));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("  ", Font));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(@"  ", InfoFont));
            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(Properties.Settings.Default.UnderCountText, InfoFont));

            //cell.Colspan = 12;
            //cell.BorderWidth = 0;
            //table.AddCell(cell);
            //#endregion

            document.Add(table);

            document.Close();
            
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            return new FileStreamResult(workStream, "application/pdf");
        }
    }
}