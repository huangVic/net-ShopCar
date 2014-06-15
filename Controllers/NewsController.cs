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
    public class NewsController : Controller
    {
        private ShopCar.Class.wfdb wf = new ShopCar.Class.wfdb();
        private string _connectionString = ConfigurationManager.ConnectionStrings["ShopCarConnectionString"].ConnectionString;

        //
        // GET: /News/

        public ActionResult NewsToJsonData()
        {
            Models.ShopCarDatasetTableAdapters.NewsTableAdapter newsadp = new Models.ShopCarDatasetTableAdapters.NewsTableAdapter();
            DataTable dt = newsadp.GetData();
            Hashtable myHT = new Hashtable();
            myHT.Add("aaData", dt);
            string obj_json = JsonConvert.SerializeObject(myHT);
            return Content(obj_json, "application/json");
        }


        //後台消息清單
        public ActionResult NewsList()
        {
            return View();
        }

        //後台消息新增畫面
        public ActionResult NewsAdd()
        {
            return View();
        }

        // 後台消息儲存
        [HttpPost]
        public ActionResult NewsCreate(FormCollection formCollection)
        {

 
            // 2. 寫入資料表
            //----> 程式碼 
            Models.ShopCarDatasetTableAdapters.NewsTableAdapter newsadp = new Models.ShopCarDatasetTableAdapters.NewsTableAdapter();
            int ret_app_ser;
            


            //ret_app_ser = Convert.ToInt32(proadp.InsertProductData(formCollection["proNo"], formCollection["proName"],price,special_price,formCollection["proClassId"],proactive,formCollection["prodFeature"],formCollection["prodDesc"]));

            ret_app_ser = Convert.ToInt32(newsadp.InsertNewsData(formCollection["News_Title"], formCollection["News_Content"]));


            // 3. 使用物件 儲存起來回丟給前端 ProEdit.cshtml  


            var NewsItem = new
            {
                appSer = ret_app_ser,
                News_Title = formCollection["News_Title"].ToString(),
                News_Content = formCollection["News_Content"].ToString()
            };


            return View("NewsEdit", NewsItem);
        }


        // 後台消息修改畫面
        public ActionResult NewsEdit(string appSer)
        {
            // 1. 取得Get 參數, app_Ser
    

            // 2. 於資料庫搜尋此筆資料
            //----> 程式碼 
            Models.ShopCarDatasetTableAdapters.NewsTableAdapter newsadp = new Models.ShopCarDatasetTableAdapters.NewsTableAdapter();
            DataTable dt;
            dt = newsadp.GetOneNewsData(Convert.ToInt32(appSer));
            DataRow drow = dt.Rows[0];


 
            var NewsItem = new
            {
                appSer = wf.tos(drow["app_ser"]),
                News_Title = wf.tos(drow["News_Title"]),
                News_Content = wf.tos(drow["News_Content"]),
                Create_Date = wf.tos(drow["Create_Date"])
            };


            return View("NewsEdit", NewsItem);
        }


        // 後台消息修改儲存
        [HttpPost]
        public ActionResult NewsUpdate(FormCollection formCollection)
        {

            // 1. 取得前端 form 的欄位資料


            // 2. 更新產品資料表
            //----> 程式碼 


            Models.ShopCarDatasetTableAdapters.NewsTableAdapter newsadp = new Models.ShopCarDatasetTableAdapters.NewsTableAdapter();

            newsadp.UpdateNewsData(formCollection["News_Title"], formCollection["News_Content"],  Convert.ToInt32(formCollection["app_ser"]));

            var NewsItem = new
            {
                appSer = formCollection["app_ser"],
                News_Title = formCollection["News_Title"],
                News_Content = formCollection["News_Content"]
            };


            return View("NewsEdit", NewsItem);
        }


        // 後台刪除消息
        [HttpPost]
        public ActionResult NewsDelete(FormCollection formCollection)
        {
            

            Int32 appser = Convert.ToInt32(formCollection["app_Ser"]);
            // 2. 刪除該appser產品
            //----> 程式碼 
            Models.ShopCarDatasetTableAdapters.NewsTableAdapter newsadp = new Models.ShopCarDatasetTableAdapters.NewsTableAdapter();
            newsadp.DeleteNewsData(appser);


            // 3. Return pro_list  
            return RedirectToAction("NewsList");
        }

    }
}
