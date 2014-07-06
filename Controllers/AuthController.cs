using ShopCar.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopCar.Controllers
{
    [WebAuthorizeFilter(AuthFlag = false)]
    public class AuthController : Controller
    {
        private string _connectionString = ConfigurationManager.ConnectionStrings["ShopCarConnectionString"].ConnectionString;

        private ShopCar.Class.wfdb wf = new ShopCar.Class.wfdb();
        //
        // GET: /Auth/

        public ActionResult Login(string msg)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> Auth Login -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> Auth msg: " + msg);

            if (msg == null)
            {
                msg = "";
            }

            var userInfo = new
            {
                authMsg = wf.tos(msg)
            };

            return View(userInfo);
        }


        [HttpPost]
        public ActionResult Auth(FormCollection formCollection)
        {

            string msg = "";

            System.Diagnostics.Debug.WriteLine(" >>>> Auth -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> email: " + formCollection["email"]);
            System.Diagnostics.Debug.WriteLine(" >>>> password: " + formCollection["password"]);

            // 1. 資料庫連線 
            SqlConnection cn = new SqlConnection(_connectionString);

            // 2. SQL 指令 
            string sql = "select * from account where emp_email=@email and login_password=@pwd";

            cn.Open();
            SqlCommand comm = new SqlCommand(sql, cn);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("@email", wf.tos(formCollection["email"]));
            comm.Parameters.AddWithValue("@pwd", wf.tos(formCollection["password"]));

            SqlDataReader reader = comm.ExecuteReader();


            if (reader.HasRows)
            {
                // 找到會員資料
                while (reader.Read())
                {
                    Session["emp_ser"] = reader["app_ser"];
                    Session["emp_name"] = reader["emp_name"];
                    Session["emp_email"] = reader["emp_email"];
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
                return RedirectToAction("Login", "Auth", new { msg = msg });
            }
            else
            {
                return RedirectToAction("OrderList","Order");
            }
        }




        public ActionResult Logout()
        {
            Session["emp_ser"] = null;
            Session["emp_name"] = null;
            Session["emp_email"] = null;
            return RedirectToAction("Login");
        }
    }
}
