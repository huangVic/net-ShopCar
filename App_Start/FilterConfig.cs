using System.Web;
using System.Web.Mvc;
using ShopCar.Filters;

namespace ShopCar
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new WebAuthorizeFilter());
        }
    }
}