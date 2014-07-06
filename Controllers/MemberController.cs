using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Collections;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ShopCar.Filters;

namespace ShopCar.Controllers
{
    [WebAuthorizeFilter(AuthFlag = true)]
    public class MemberController : Controller
    {
        private ShopCar.Class.wfdb wf = new ShopCar.Class.wfdb();
        private string _connectionString = ConfigurationManager.ConnectionStrings["ShopCarConnectionString"].ConnectionString;
        
        //
        // GET: /Member/

        public ActionResult MemberToJsonData()
        {
            Models.ShopCarDatasetTableAdapters.MemberTableAdapter memadp = new Models.ShopCarDatasetTableAdapters.MemberTableAdapter();
            DataTable dt = memadp.GetData();
            Hashtable myHT = new Hashtable();
            myHT.Add("memberItem", dt);
            string obj_json = JsonConvert.SerializeObject(myHT);
            return Content(obj_json, "application/json");
        }


        //後台會員清單
        public ActionResult MemberList()
        {
            return View();
        }

        // 後台會員修改畫面
        public ActionResult MemberEdit(string appSer)
        {
            // 1. 取得Get 參數, app_Ser
            System.Diagnostics.Debug.WriteLine(" >>>> ProEdit AppSer: " + appSer);

            // 2. 於資料庫搜尋此筆資料
            //----> 程式碼 
            Models.ShopCarDatasetTableAdapters.MemberTableAdapter memadp = new Models.ShopCarDatasetTableAdapters.MemberTableAdapter();
            DataTable dt;
            dt = memadp.GetOneMemberData(Convert.ToInt32(appSer));
            DataRow drow = dt.Rows[0];


            //male 性別直接以男/女表示
     
            var memItem = new
            {
                appSer = wf.tos(drow["app_ser"]),
                user_id = wf.tos(drow["user_id"]),
                user_name = wf.tos(drow["user_name"]),
                user_password = wf.tos(drow["user_password"]),
                birthDay = wf.tos(((DateTime)drow["BirthDay"]).ToString("yyyy/MM/dd")),
                male = wf.tos(drow["male"]),
                mobile = wf.tos(drow["mobile"]),
                tel = wf.tos(drow["tel"]),
                extno = wf.tos(drow["extno"]),
                address = wf.tos(drow["Address"]),
                email = wf.tos(drow["email"])
            };

            return Json(memItem, JsonRequestBehavior.AllowGet);
            //return View("MemberEdit", memItem);
        }


        // 後台會員修改儲存
        [HttpPost]
        public ActionResult MemberUpdate(FormCollection formCollection)
        {

            // 1. 取得前端 form 的欄位資料


            // 2. 更新產品資料表
            //----> 程式碼 
            Nullable<DateTime> BirthDay;
            if (formCollection["BirthDay"] == "")
            { BirthDay = null; }
            else { BirthDay = Convert.ToDateTime(formCollection["BirthDay"]); }

            Models.ShopCarDatasetTableAdapters.MemberTableAdapter memadp = new Models.ShopCarDatasetTableAdapters.MemberTableAdapter();

            memadp.UpdateMemberData(formCollection["user_id"], formCollection["user_name"], formCollection["user_password"], BirthDay, formCollection["male"], formCollection["mobile"], formCollection["tel"], formCollection["extno"], formCollection["Address"], formCollection["email"], Convert.ToInt32(formCollection["app_ser"]));

            /*
            var memItem = new
            {
                appSer = formCollection["app_ser"],
                user_id = formCollection["user_id"],
                user_name = formCollection["user_name"],
                user_password = formCollection["user_password"],
                BirthDay = formCollection["BirthDay"],
                male = formCollection["male"],
                mobile = formCollection["mobile"],
                tel = formCollection["tel"],
                extno = formCollection["extno"],
                Address = formCollection["Address"],
                email = formCollection["email"]
            };
            */

           // return View("MemberEdit", memItem);
            return RedirectToAction("MemberList");
        }


        // 後台刪除會員
        [HttpPost]
        public ActionResult MemberDelete(FormCollection formCollection)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> ProDelete -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> ProAdd proNo: " + formCollection["appSer"]);

            Int32 appser = Convert.ToInt32(formCollection["appSer"]);
            // 2. 刪除該appser產品
            //----> 程式碼 
            Models.ShopCarDatasetTableAdapters.MemberTableAdapter memadp = new Models.ShopCarDatasetTableAdapters.MemberTableAdapter();
            memadp.DeleteMemberData(appser);


            // 3. Return pro_list  
            return RedirectToAction("MemberList");
        }

    }
}
