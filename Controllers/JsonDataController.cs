using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections;

namespace ShopCar.Controllers
{
    public class JsonDataController : Controller
    {
        private string _connectionString = ConfigurationManager.ConnectionStrings["ShopCarConnectionString"].ConnectionString;
        // GET: /JsonData/

        public ActionResult ProToJsonData()
        {
            // 1. 資料庫連線 
            SqlConnection cn = new SqlConnection(_connectionString);

            // 2. SQL 指令 
            string sql = "select app_ser,ProductID as prod_no,ProName as prod_name,'' as prod_desc,SalePrice as prod_price,'' as prod_special_price,'' as prod_feature,'' as prod_class , pro_active from product";
            //string sql = "select app_ser,ProductID,ProName,'' as prod_desc,SalePrice,'' as prod_special_price,'' as prod_feature,'' as prod_class from product";
            SqlCommand comm = new SqlCommand(sql, cn);
            
            // 2. SqlDataAdapter & DataSet 


            DataSet ds = new DataSet();
            using (cn)
            {
                SqlDataAdapter adapter = new SqlDataAdapter(comm);
                adapter.Fill(ds);
                DataTable myDataTable = ds.Tables[0];
                cn.Close();
                //Models.ShopCarDatasetTableAdapters.ProductTableAdapter proadp = new Models.ShopCarDatasetTableAdapters.ProductTableAdapter();
                //DataTable table = proadp.GetData();
                Hashtable myHT = new Hashtable();
                myHT.Add("aaData", myDataTable);
                string obj_json = JsonConvert.SerializeObject(myHT);
                return Content(obj_json, "application/json");

            }
           



            //string jsonData = System.IO.File.ReadAllText(Server.MapPath("~/json/porduction.json"));
            
            //return Content(jsonData, "application/json");
        }

    }
}
