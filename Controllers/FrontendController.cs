using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ShopCar.Controllers
{
    public class FrontendController : Controller
    {
        private ShopCar.Class.wfdb wf = new ShopCar.Class.wfdb();

        //
        // GET: /Frontend/


        // 產品介紹
        public ActionResult Product()
        {
            return View();
        }


        // 會員中心 - 新增會員
        public ActionResult MemberAdd()
        {
            return View();
        }

        [HttpPost]
        // 會員中心 - 填寫會員基本資料
        public ActionResult MemberEdit(FormCollection formCollection)
        {

            System.Diagnostics.Debug.WriteLine(" >>>> MemberEdit -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> email: " + formCollection["user_email"]);
            System.Diagnostics.Debug.WriteLine(" >>>> password: " + formCollection["password"]);

            string ps_encode = "";
            if (wf.tos(formCollection["password"]) != "") {
                ps_encode = wf.base64Encode(formCollection["password"]);
            }

            var userInfo = new
            {
                user_email = formCollection["user_email"],
                ps = ps_encode
            };
            return View(userInfo);
        }



        [HttpPost]
        // 會員中心 - 會員資料 insert db
        public ActionResult MemberCreate(FormCollection formCollection)
        {

            System.Diagnostics.Debug.WriteLine(" >>>> MemberEdit -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> user_id: " + formCollection["user_id"]);
            System.Diagnostics.Debug.WriteLine(" >>>> user_password: " + formCollection["user_password"]);
            System.Diagnostics.Debug.WriteLine(" >>>> user_name: " + formCollection["user_name"]);
            System.Diagnostics.Debug.WriteLine(" >>>> male: " + formCollection["male"]);
            System.Diagnostics.Debug.WriteLine(" >>>> BirthDay: " + formCollection["BirthDay"]);
            System.Diagnostics.Debug.WriteLine(" >>>> tel: " + formCollection["tel"]);
            System.Diagnostics.Debug.WriteLine(" >>>> extno: " + formCollection["extno"]);
            System.Diagnostics.Debug.WriteLine(" >>>> mobile: " + formCollection["mobile"]);
            System.Diagnostics.Debug.WriteLine(" >>>> email: " + formCollection["email"]);
            System.Diagnostics.Debug.WriteLine(" >>>> Address: " + formCollection["Address"]);

            if(wf.tos(formCollection["user_id"]) == "" || wf.tos(formCollection["user_password"]) == ""){
                // 非法進入此 action 重導到新增會員畫面
                return RedirectToAction("MemberAdd");
            }else{
                // 將會員資料寫入資料庫




                // 寫入成功後到新增成功畫面
                return RedirectToAction("MemberCreateSuccess");
            }
              
        }


        // 會員中心 - 會員新增成功
        public ActionResult MemberCreateSuccess()
        {
            return View();
        }
    }
}
