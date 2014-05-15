using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopCar.Controllers
{
    public class FrontendController : Controller
    {
        //
        // GET: /Frontend/

        public ActionResult Product()
        {
            return View();
        }


        public ActionResult ProductItem(string proAppSer)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> ProductItem -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> ProductItem app_ser: " + proAppSer);

            var info = new
            {
                app_ser = proAppSer
            };
            return View("ProductItem", info);
        }
    }
}
