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
    public class FrontendController : Controller
    {
        
        private string _connectionString = ConfigurationManager.ConnectionStrings["ShopCarConnectionString"].ConnectionString;
        
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
    }
}
