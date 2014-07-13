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
using ShopCar.Models;
using ShopCar.Mongodb;

namespace ShopCar.Controllers
{
    [WebAuthorizeFilter(AuthFlag = false)]
    public class FrontendController : Controller, IDisposable
    {
        
        private string _connectionString = ConfigurationManager.ConnectionStrings["ShopCarConnectionString"].ConnectionString;
        private ShopCar.Class.wfdb wf = new ShopCar.Class.wfdb();

        private Dao mongoDao = new Dao();
        private bool disposed = false;


        //
        // GET: /mongoDao/

        public ActionResult getPreOrderList()
        {

            Hashtable myHT = new Hashtable();
            myHT.Add("preOrderList", mongoDao.GetAllPreOder());
            string obj_json = JsonConvert.SerializeObject(myHT);
            return Content(obj_json, "application/json");
        }

        [HttpPost]
        // 加入購物車
        public ActionResult addProductToCar(FormCollection formCollection)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> addProductToCar -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> app_ser: " + formCollection["app_ser"]);
            System.Diagnostics.Debug.WriteLine(" >>>> pro_class_name: " + formCollection["pro_class_name"]);
            System.Diagnostics.Debug.WriteLine(" >>>> prod_desc: " + formCollection["prod_desc"]);
            System.Diagnostics.Debug.WriteLine(" >>>> prod_feature: " + formCollection["prod_feature"]);
            System.Diagnostics.Debug.WriteLine(" >>>> prod_name: " + formCollection["prod_name"]);
            System.Diagnostics.Debug.WriteLine(" >>>> prod_no: " + formCollection["prod_no"]);
            System.Diagnostics.Debug.WriteLine(" >>>> prod_price: " + formCollection["prod_price"]);
            System.Diagnostics.Debug.WriteLine(" >>>> prod_special_price: " + formCollection["prod_special_price"]);

            PreOrderModel pom = new PreOrderModel();
            pom.app_ser = wf.toi(formCollection["app_ser"]);
            pom.pro_class_name = wf.tos(formCollection["pro_class_name"]);
            pom.prod_desc = wf.tos(formCollection["prod_desc"]);
            pom.prod_feature = wf.tos(formCollection["prod_feature"]);
            pom.prod_name = wf.tos(formCollection["prod_name"]);
            pom.prod_no = wf.tos(formCollection["prod_no"]);
            pom.prod_price = wf.toi(formCollection["prod_price"]);
            pom.prod_special_price = wf.toi(formCollection["prod_special_price"]);
            pom.create_date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            pom.update_date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            pom.user_app_ser = wf.toi(Session["app_ser"]);
            pom.user_id = wf.tos(Session["user_id"]);
            pom.user_name = wf.tos(Session["user_name"]);

            mongoDao.CreatePreOrder(pom);

            Hashtable myHT = new Hashtable();
            myHT.Add("preOrderList", mongoDao.GetAllPreOder());
            string obj_json = JsonConvert.SerializeObject(myHT);
            return Content(obj_json, "application/json");
        }


        // 產品介紹
        public ActionResult Product()
        {
            return View();
        }


        // 會員中心 - 會員中心
        public ActionResult Member()
        {
            System.Diagnostics.Debug.WriteLine(" >>>> Member -------------------------->>>> ");


            MemberInfo userInfo = new MemberInfo();


            if (Session["app_ser"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(" >>>>  session user_id --->>>  " + Session["user_id"]);
                System.Diagnostics.Debug.WriteLine(" >>>>  session user_name --->>>  " + Session["user_name"]);
                System.Diagnostics.Debug.WriteLine(" >>>>  session app_ser --->>>  " + Session["app_ser"]);

                // 1. 資料庫連線 
                SqlConnection cn = new SqlConnection(_connectionString);

                // 2. SQL 指令 
                string sql = "select * from member where app_ser=@app_ser";

                cn.Open();
                SqlCommand comm = new SqlCommand(sql, cn);
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("@app_ser", wf.tos(Session["app_ser"]));

                SqlDataReader reader = comm.ExecuteReader();
                if (reader.HasRows)
                {
                    // 找到會員資料
                    while (reader.Read())
                    {
                         userInfo.app_ser = wf.tos(reader["app_ser"]);
                         userInfo.user_id = wf.tos(reader["user_id"]);
                         userInfo.user_name = wf.tos(reader["user_name"]);
                         userInfo.BirthDay = wf.tos(reader["BirthDay"]);
                         userInfo.mobile = wf.tos(reader["mobile"]);
                         userInfo.tel = wf.tos(reader["tel"]);
                         userInfo.Address = wf.tos(reader["Address"]);
                         userInfo.email = wf.tos(reader["email"]);
                    }

                }
        
             }


            return View(userInfo);
        }



        public ActionResult Logout()
        {
            Session["user_id"] = null;
            Session["user_name"] = null;
            Session["app_ser"] = null;

            return RedirectToAction("Member");
        }

        // 會員中心 - 會員登入
        public ActionResult Login(string msg)
        {

            System.Diagnostics.Debug.WriteLine(" >>>> Login -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> msg: " + msg);

            if (msg == null) {
                msg = "";
            }

            var userInfo = new
            {
                authMsg = wf.tos(msg)
            };

            return View(userInfo);
        }

        [HttpPost]
        // 會員中心 - 會員登入驗證
        public ActionResult Auth(FormCollection formCollection)
        {
            string msg = "";

            System.Diagnostics.Debug.WriteLine(" >>>> Auth -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> email: " + formCollection["user_id"]);
            System.Diagnostics.Debug.WriteLine(" >>>> password: " + formCollection["user_password"]);

            // 1. 資料庫連線 
            SqlConnection cn = new SqlConnection(_connectionString);

            // 2. SQL 指令 
            string sql = "select * from member where user_id=@id and user_password=@pwd";

            cn.Open();
            SqlCommand comm = new SqlCommand(sql, cn);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("@id", wf.tos(formCollection["user_id"]));
            comm.Parameters.AddWithValue("@pwd", wf.tos(formCollection["user_password"]));

            SqlDataReader reader = comm.ExecuteReader();


            if (reader.HasRows)
            {
                // 找到會員資料
                while (reader.Read())
                {
                    Session["user_id"] = reader["user_id"];
                    Session["user_name"] = reader["user_name"];
                    Session["app_ser"] = reader["app_ser"]; 
                }

            }
            else
            {
                // 查無會員資訊
                msg = "帳號密碼錯誤!!";
                
            }

            cn.Close();
            cn.Dispose();


            if (msg != "")
            {
                return RedirectToAction("Login", "Frontend", new { msg = msg });
            }
            else 
            {
                return RedirectToAction("Member");
            }
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
            System.Diagnostics.Debug.WriteLine(" >>>> db null: " + Convert.IsDBNull(formCollection["user_password"]));
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
                Models.ShopCarDatasetTableAdapters.MemberTableAdapter memadp = new Models.ShopCarDatasetTableAdapters.MemberTableAdapter();
                int ret_app_ser;

                string ps_decode = "";
                if (Convert.IsDBNull(formCollection["password"]) == false)
                {
                    ps_decode = wf.base64Decode(formCollection["user_password"]);
                }


                ret_app_ser = Convert.ToInt32(memadp.InsertMemberData(formCollection["user_id"], formCollection["user_name"], ps_decode , Convert.ToDateTime(formCollection["BirthDay"]), formCollection["male"], formCollection["mobile"], formCollection["tel"], formCollection["extno"], formCollection["Address"], formCollection["email"]));

                // 寫入成功後到新增成功畫面
                return RedirectToAction("MemberCreateSuccess");
            }
              
        }


        // 會員中心 - 會員新增成功
        public ActionResult MemberCreateSuccess()
        {
            return View();
        }

        //會員是否存在
        public ActionResult IDExistMember(string user_id)
        {
            string msg;

            // 1. 取得Get 參數, app_Ser
            System.Diagnostics.Debug.WriteLine(" >>>> IDExistMember user_id: " + user_id);

            // 1. 資料庫連線 
            SqlConnection cn = new SqlConnection(_connectionString);

            // 2. SQL 指令 
            string sql = "select COUNT(*) from member where user_id=@id";

            cn.Open();
            SqlCommand comm = new SqlCommand(sql, cn);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("@id", user_id);


            if ((int)comm.ExecuteScalar() > 0)
            {
                msg = "此帳號已被註冊,請重新輸入帳號!!"; //id 已存在會員檔,不能新增
            }
            else
            {
                msg = "OK";
            }

            cn.Close();
            cn.Dispose();

            var result = new
            {
                message = msg
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // 關於馨隆
        public ActionResult AboutSinLong()
        {
            return View();
        }

        // 經營理念
        public ActionResult MissionStatement() 
        {
            return View();
        }


        // 最新消息
        public ActionResult News()
        {
            return View();
        }


        // 常見問題 - 會員問題
        public ActionResult QandA()
        {
            return View();
        }

        // 常見問題 - 訂購問題
        public ActionResult OrderQa()
        {
            return View();
        }

        // 常見問題 - 付款問題
        public ActionResult PayingQa()
        {
            return View();
        }

        // 聯絡我們
        public ActionResult ContactUs(string msg)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> ContactUs -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> msg: " + msg);

            if (msg == null)
            {
                msg = "";
            }

            var obj = new
            {
                retMsg = wf.tos(msg)
            };

            return View(obj);
        }





        // 產品分類資料
        public ActionResult GetProClassList()
        {


            // 1. 取得所有分類
            //----->
            Models.ShopCarDatasetTableAdapters.Product_ClassTableAdapter classadp = new Models.ShopCarDatasetTableAdapters.Product_ClassTableAdapter();
            DataTable dt = classadp.GetData();

            // 2. 將資料塞到 arraylist
            ArrayList list = new ArrayList();

            foreach (DataRow drow in dt.Rows)
            {
                var prodClass = new
                {
                    app_ser = wf.tos(drow["app_ser"]),
                    pro_class_no = wf.tos(drow["pro_class_no"]),
                    pro_class_name = wf.tos(drow["pro_class_name"]),
                    pro_class_desc = wf.tos(drow["pro_class_desc"]),
                    class_active = wf.tos(drow["class_active"])
                };
                list.Add(prodClass);

            }

            // 3. 將 arraylist 放到Hashtable
            Hashtable myHT = new Hashtable();
            myHT.Add("classList", list);

            // 3. 回傳json
            return Json(myHT, JsonRequestBehavior.AllowGet);
        }


        // 取得某產品分類下所有被啟用的產品列表 Api
        public ActionResult getProductData(string appSer)
        {
            System.Diagnostics.Debug.WriteLine("[Get] GetProductData AppSer >>> " + appSer);

            // 將 arrayList 掛到 files 底下, 並轉成json回傳前端
            Hashtable myHT = new Hashtable();


            var userInfo = new
            {
                user_id= wf.tos(Session["user_id"]),
                user_name = wf.tos(Session["user_name"]),
                app_ser = wf.tos(Session["app_ser"])
            };
            myHT.Add("userInfo", userInfo);

            // 1. 先撈資料庫 File Table 的資料 (SQL)
            //----> 程式碼 
            Models.ShopCarDatasetTableAdapters.Product_PhotoTableAdapter photoadp = new Models.ShopCarDatasetTableAdapters.Product_PhotoTableAdapter();
            DataTable dt = photoadp.GetPhotoList(Convert.ToInt32(appSer));


            ArrayList list = new ArrayList();
            // 2. 使用迴圈將每一筆 row 取出
            foreach (DataRow drow in dt.Rows)
            {
                var fileItem = new ViewDataUploadFilesResult();
                fileItem.name = wf.tos(drow["file_name"]);
                fileItem.size = wf.toi(drow["file_size"]);
                fileItem.contentType = wf.tos(drow["content_type"]);
                fileItem.url = wf.tos(drow["download_url"]);
                fileItem.deleteUrl = wf.tos(drow["delete_url"]); //"/Product/Delete?id=" + appSer; // file table的 App_Ser
                // fileItem.thumbnailUrl = @"data:image/png;base64," + EncodeFile(wf.tos(drow["thumbnail_url"]));
                fileItem.deleteType = wf.tos(drow["delete_type"]);
                list.Add(fileItem);
            }
            myHT.Add("files", list);


            // 1. 資料庫連線 
            SqlConnection cn = new SqlConnection(_connectionString);

            // 2. SQL 指令 
            string sql = "select a.app_ser,a.ProductID as prod_no,a.ProName as prod_name,a.pro_desc as prod_desc, a.prod_price, a.prod_special_price, a.prod_feature, b.pro_class_name from product a left join Product_Class b on a.prod_class_id=b.app_ser where a.pro_active='on' and a.app_ser=@id";
            //string sql = "select app_ser,ProductID,ProName,'' as prod_desc,SalePrice,'' as prod_special_price,'' as prod_feature,'' as prod_class from product";
            cn.Open();
            SqlCommand comm = new SqlCommand(sql, cn);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("@id", appSer);


            // 2. SqlDataAdapter & DataSet 
            DataSet ds = new DataSet();
            using (cn)
            {
                SqlDataAdapter adapter = new SqlDataAdapter(comm);
                adapter.Fill(ds);
                DataTable myDataTable = ds.Tables[0];
                cn.Close();
                cn.Dispose();

                myHT.Add("productItem", myDataTable);

                string obj_json = JsonConvert.SerializeObject(myHT);
                return Content(obj_json, "application/json");

            }
        }

        // 取得某產品分類下所有被啟用的產品列表 Api
        public ActionResult ClassToActiveProductData(int classAppSer)
        {

            if (Convert.IsDBNull(classAppSer))
            {
                classAppSer = 1;
            }

            // 1. 資料庫連線 
            SqlConnection cn = new SqlConnection(_connectionString);

            // 2. SQL 指令 
            string sql = "select a.app_ser,a.ProductID as prod_no,a.ProName as prod_name,a.pro_desc as prod_desc, a.prod_price,a.prod_special_price,a.prod_feature,isnull((select top 1 download_url from ShopCar.dbo.Product_Photo c where a.app_ser=c.app_ser),'') as download_url from product a left join Product_Class b on a.prod_class_id=b.app_ser where a.pro_active='on' and b.app_ser=@id";
            //string sql = "select app_ser,ProductID,ProName,'' as prod_desc,SalePrice,'' as prod_special_price,'' as prod_feature,'' as prod_class from product";
            cn.Open();
            SqlCommand comm = new SqlCommand(sql, cn);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("@id", classAppSer);


            // 2. SqlDataAdapter & DataSet 
            DataSet ds = new DataSet();
            using (cn)
            {
                SqlDataAdapter adapter = new SqlDataAdapter(comm);
                adapter.Fill(ds);
                DataTable myDataTable = ds.Tables[0];
                cn.Close();
                cn.Dispose();
                //Models.ShopCarDatasetTableAdapters.ProductTableAdapter proadp = new Models.ShopCarDatasetTableAdapters.ProductTableAdapter();
                //DataTable table = proadp.GetData();
                Hashtable myHT = new Hashtable();
                myHT.Add("productList", myDataTable);
                string obj_json = JsonConvert.SerializeObject(myHT);
                return Content(obj_json, "application/json");

            }
        }



        public ActionResult NewsToJsonData()
        {
            Models.ShopCarDatasetTableAdapters.NewsTableAdapter newsadp = new Models.ShopCarDatasetTableAdapters.NewsTableAdapter();
            DataTable dt = newsadp.GetData();
            Hashtable myHT = new Hashtable();
            myHT.Add("newsList", dt);
            string obj_json = JsonConvert.SerializeObject(myHT);
            return Content(obj_json, "application/json");
        }
    }
}


public class MemberInfo
{
    public string app_ser { get; set; }
    public string user_id { get; set; }
     public string user_name { get; set; }
    public string BirthDay { get; set; }
    public string mobile { get; set; }
    public string tel { get; set; }
    public string Address { get; set; }
    public string email { get; set; }  
   
}