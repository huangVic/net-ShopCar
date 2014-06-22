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

namespace ShopCar.Controllers
{
    public class ContactController : Controller
    {
       

        private ShopCar.Class.wfdb wf = new ShopCar.Class.wfdb();
        private string _connectionString = ConfigurationManager.ConnectionStrings["ShopCarConnectionString"].ConnectionString;

        //
        // GET: /Contact/

        public ActionResult ConToJsonData()
        {
            Models.ShopCarDatasetTableAdapters.ContactTableAdapter conadp = new Models.ShopCarDatasetTableAdapters.ContactTableAdapter();
            DataTable dt = conadp.GetData();
            Hashtable myHT = new Hashtable();
            myHT.Add("aaData", dt);
            string obj_json = JsonConvert.SerializeObject(myHT);
            return Content(obj_json, "application/json");
        }


        //後台留言清單
        public ActionResult ConList()
        {
            return View();
        }

        //前台留言後顯示畫面
        public ActionResult ConShow()
        {
            return View();
        }

        //前台留言新增畫面
        public ActionResult ConAdd()
        {
            return View();
        }

        

        // 前台留言儲存
        [HttpPost]
        public ActionResult ConCreate(FormCollection formCollection)
        {

            string msg = "";

            // 2. 寫入資料表
            //----> 程式碼 
             Models.ShopCarDatasetTableAdapters.ContactTableAdapter conadp = new Models.ShopCarDatasetTableAdapters.ContactTableAdapter();
            int ret_app_ser;



            //ret_app_ser = Convert.ToInt32(proadp.InsertProductData(formCollection["proNo"], formCollection["proName"],price,special_price,formCollection["proClassId"],proactive,formCollection["prodFeature"],formCollection["prodDesc"]));

            ret_app_ser = Convert.ToInt32(conadp.InsertContactData(formCollection["guest_name"], formCollection["guest_mobile"], formCollection["email"], formCollection["title"], formCollection["suggestion"]));


            // 3.  

            if (wf.toi(ret_app_ser) > 0)
            {
                msg = "1";
            }


            /**
            var ConItem = new
            {
                appSer = ret_app_ser,
                guest_name = formCollection["guest_name"],
                guest_mobile = formCollection["guest_mobile"],
                email = formCollection["email"],
                title = formCollection["title"],
                suggestion = formCollection["suggestion"]
            };
             */

            return RedirectToAction("ContactUs", "Frontend", new { msg = msg });
            //return View("ConShow", ConItem);
        }


        // 後台留言修改畫面(顧客留言應該是不用修改此處忽略

        //public ActionResult NewsEdit(string appSer)
        //{
        //    // 1. 取得Get 參數, app_Ser


        //    // 2. 於資料庫搜尋此筆資料
        //    //----> 程式碼 
        //     Models.ShopCarDatasetTableAdapters.ContactTableAdapter conadp = new Models.ShopCarDatasetTableAdapters.ContactTableAdapter();
        //    DataTable dt;
        //    dt = conadp.GetOneNewsData(Convert.ToInt32(appSer));
        //    DataRow drow = dt.Rows[0];


        //    //male 性別直接以男/女表示

        //    var ConItem = new
        //    {
        //        appSer = wf.tos(drow["app_ser"]),
        //        News_Title = wf.tos(drow["News_Title"]),
        //        News_Content = wf.tos(drow["News_Content"]),
        //        Create_Date = wf.tos(drow["Create_Date"])
        //    };


        //    return View("NewsEdit", ConItem);
        //}


        // 後台消息修改儲存
        //[HttpPost]
        //public ActionResult NewsUpdate(FormCollection formCollection)
        //{

        //    // 1. 取得前端 form 的欄位資料


        //    // 2. 更新產品資料表
        //    //----> 程式碼 


        //     Models.ShopCarDatasetTableAdapters.ContactTableAdapter conadp = new Models.ShopCarDatasetTableAdapters.ContactTableAdapter();

        //    conadp.UpdateNewsData(formCollection["News_Title"], formCollection["News_Content"], Convert.ToInt32(formCollection["app_ser"]));

        //    var ConItem = new
        //    {
        //        appSer = formCollection["app_ser"],
        //        News_Title = formCollection["News_Title"],
        //        News_Content = formCollection["News_Content"]
        //    };


        //    return View("NewsEdit", ConItem);
        //}


        // 後台留言刪除
        [HttpPost]
        public ActionResult NewsDelete(FormCollection formCollection)
        {


            Int32 appser = Convert.ToInt32(formCollection["app_Ser"]);
            // 2. 刪除該appser產品
            //----> 程式碼 
             Models.ShopCarDatasetTableAdapters.ContactTableAdapter conadp = new Models.ShopCarDatasetTableAdapters.ContactTableAdapter();
            conadp.DeleteContactData(appser);


            // 3. Return pro_list  
            return RedirectToAction("ConList");
        }

    }
}
