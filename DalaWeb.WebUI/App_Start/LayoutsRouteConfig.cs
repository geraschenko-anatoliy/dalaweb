﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NavigationRoutes;
using DalaWeb.WebUI.Controllers;

namespace DalaWeb.WebUI
{
    public class LayoutsRouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapNavigationRoute<AbonentController>("Абоненты", c => c.Index());

            routes.MapNavigationRoute<AddressController>("Адреса", c => c.Delete(0))
                .AddChildRoute<AddressController>("Полный список", c => c.Index())
                .AddChildRoute<CityController>("Города", c => c.Index())
                .AddChildRoute<StreetController>("Улицы", c => c.Index());

            routes.MapNavigationRoute<AbonentCreditController>("Кредиты", c => c.Delete(0))
                .AddChildRoute<CreditController>("Кредиты", c => c.Index())
                .AddChildRoute<AbonentCreditController>("Абоненты", c => c.Index());

            routes.MapNavigationRoute<ServiceCompanyController>("Сервисы", c => c.Details(0))
                .AddChildRoute<ServiceCompanyController>("Компании", c => c.Index())
                .AddChildRoute<ServiceController>("Сервисы", c => c.Index())
                .AddChildRoute<TariffController>("Тарифы", c => c.Index())
                .AddChildRoute<AbonentServiceController>("Абоненты", c => c.Index());

            routes.MapNavigationRoute<CounterController>("Счетчики", c => c.Delete(0))
                .AddChildRoute<CounterController>("Счетчики", c => c.Index())
                .AddChildRoute<CounterValuesController>("Показания", c => c.Index())
                .AddChildRoute<StampController>("Пломбы", c => c.Index());

            routes.MapNavigationRoute<PaymentController>("Оплата", c => c.Delete(0))
                .AddChildRoute<PaymentController>("Квитанции", c => c.Receipts())
                .AddChildRoute<PaymentController>("Платежи", c => c.Index())
                .AddChildRoute<PaymentController>("Массовое пополнение", c => c.NumerousRefill())
                .AddChildRoute<SettingController>("Подпись счета", c => c.Index());

            routes.MapNavigationRoute<PaymentQueueController>("Статистика", c => c.Delete(0))
                .AddChildRoute<StatisticController>("Услуги", c => c.Index())
                .AddChildRoute<StatisticController>("Касса", c => c.DailyPayments())
                .AddChildRoute<StatisticController>("Отключение", c=> c.ListOfDebtors())
                .AddChildRoute<PaymentQueueController>("Очередь", c => c.Index());
        }
    }
}
