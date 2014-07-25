using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
//using BootstrapMvcSample.Controllers;
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

            routes.MapNavigationRoute<AbonentCreditController>("Кредиты", c => c.Delete(0, 0))
                .AddChildRoute<CreditController>("Кредиты", c => c.Index())
                .AddChildRoute<AbonentCreditController>("Пользователи", c => c.Index());

            routes.MapNavigationRoute<ServiceCompanyController>("Сервисы", c => c.Delete(0))
                .AddChildRoute<ServiceCompanyController>("Компании", c => c.Index())
                .AddChildRoute<ServiceController>("Сервисы", c => c.Index())
                .AddChildRoute<AbonentServiceController>("Пользователи", c => c.Index());

        }
    }
}
