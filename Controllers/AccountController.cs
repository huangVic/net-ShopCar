using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ShopCar.Controllers
{
    public class AccountController : Controller
    {
        private ShopCar.Class.wfdb wf = new ShopCar.Class.wfdb();
        //
        // GET: /Account/EmpList

        public ActionResult EmpList()
        {
            return View();
        }


        // 取得所有員工列表
        // GET: /Account/GetAllEmpList

        public ActionResult GetAllEmpList()
        {
            string title;
            string status;

            Models.ShopCarDatasetTableAdapters.accountTableAdapter empadp = new Models.ShopCarDatasetTableAdapters.accountTableAdapter();
            DataTable dt = empadp.GetData();

            

            // 2. 將資料塞到 arraylist
            ArrayList list = new ArrayList();

            foreach (DataRow drow in dt.Rows)
            {
                          
                
                if (wf.tos(drow["emp_title"]) == "0")
                {
                    title = "管理者";
                }
                else { title = "一般員工";}

                if (wf.tos(drow["app_status"]) == "100")
                {
                    status = "啟用";
                }
                else { status = "關閉"; }


                var empClass = new
                {
                    app_ser = wf.tos(drow["app_ser"]),
                    emp_id = wf.tos(drow["emp_id"]),
                    emp_name = wf.tos(drow["emp_name"]),
                    emp_email = wf.tos(drow["emp_email"]),
                    emp_tel = wf.tos(drow["emp_tel"]),
                    creation_date = ((DateTime)drow["creation_date"]).ToString("yyyy/MM/dd"),
                    emp_title = title,
                    app_status = status   // 請轉換成中文; 100:啟用, 0:關閉
                };
                list.Add(empClass);
            
            }

            // 3. 將 arraylist 放到Hashtable
            Hashtable myHT = new Hashtable();
            myHT.Add("empList", list);


            // 3. 回傳json
            return Json(myHT, JsonRequestBehavior.AllowGet);
            
        }


        // 取得單一員工資料
        public ActionResult GetEmpInfo(string app_ser)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> GetEmpInfo -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> app_ser : " + app_ser);

            string title;
            

            Models.ShopCarDatasetTableAdapters.accountTableAdapter empadp = new Models.ShopCarDatasetTableAdapters.accountTableAdapter();
            DataTable dt = empadp.GetOneEmpData(Convert.ToInt32(app_ser));

            DataRow drow = dt.Rows[0];

            if (wf.tos(drow["emp_title"]) == "0")
            {
                title = "管理者";
            }
            else { title = "一般員工"; }

            /*
            if (wf.tos(drow["app_status"]) == "100")
            {
                status = "啟用";
            }
            else { status = "關閉"; }
            */


            var empinfo = new
            {
                app_ser = wf.tos(drow["app_ser"]),
                emp_id = wf.tos(drow["emp_id"]),
                emp_name = wf.tos(drow["emp_name"]),
                emp_email = wf.tos(drow["emp_email"]),
                emp_tel = wf.tos(drow["emp_tel"]),
                creation_date = ((DateTime)drow["creation_date"]).ToString("yyyy/MM/dd"),
                emp_title = title,
                login_password = wf.tos(drow["login_password"]),
                app_status = wf.tos(drow["app_status"])   // Vic: 不需轉換
            };


            return Json(empinfo, JsonRequestBehavior.AllowGet);

        }

        //
        // GET: /Account/EmpAdd

        public ActionResult EmpAdd()
        {
            return View();
        }

        //
        // PST: /Account/EmpCreate
        [HttpPost]
        public ActionResult EmpCreate(FormCollection formCollection)
        {
             // 1. 取得formCollection資訊
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate emp_id: " + formCollection["emp_id"]);
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate emp_name: " + formCollection["emp_name"]);
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate emp_email: " + formCollection["emp_email"]);
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate emp_tel: " + formCollection["emp_tel"]);
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate emp_title: " + formCollection["emp_title"]); // 0: 管理者, 1: 一般員工
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate app_status: " + formCollection["app_status"]); // 後端儲存時預設給100
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate login_password: " + formCollection["login_password"]);

            Models.ShopCarDatasetTableAdapters.accountTableAdapter empadp = new Models.ShopCarDatasetTableAdapters.accountTableAdapter();
            empadp.InsertEmpData(formCollection["emp_id"],formCollection["emp_name"], formCollection["emp_email"], formCollection["emp_tel"], formCollection["emp_title"], formCollection["login_password"]);

            return RedirectToAction("EmpList");
        }




        // 刪除員工
        [HttpPost]
        public ActionResult EmpDelete(FormCollection formCollection)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> EmpDelete -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> EmpDelete app_ser: " + formCollection["app_ser"]);

            Int32 appser = Convert.ToInt32(formCollection["app_ser"]);
            // 1. 刪除該appser產品
            //----> 程式碼 

            Models.ShopCarDatasetTableAdapters.accountTableAdapter empadp = new Models.ShopCarDatasetTableAdapters.accountTableAdapter();
            empadp.DeleteEmpData(appser);


            // 2. Return pro_list  
            return RedirectToAction("EmpList");
        }



        // 更新員工資料
        [HttpPost]
        public ActionResult EmpUpdate(FormCollection formCollection)
        {
            // 1. 取得formCollection資訊
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate app_ser: " + formCollection["app_ser"]);
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate emp_name: " + formCollection["emp_name"]);
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate emp_email: " + formCollection["emp_email"]);
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate emp_tel: " + formCollection["emp_tel"]);
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate emp_title: " + formCollection["emp_title"]); // 0: 管理者, 1: 一般員工
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate app_status: " + formCollection["app_status"]);
            System.Diagnostics.Debug.WriteLine(" >>>> EmpUpdate login_password: " + formCollection["login_password"]);

            string status = "100" ;
            if (string.IsNullOrEmpty(formCollection["app_status"]))
            {
                status = "0";
            }

            // 2. 將欄位存到資料庫
            Models.ShopCarDatasetTableAdapters.accountTableAdapter empadp = new Models.ShopCarDatasetTableAdapters.accountTableAdapter();
            empadp.UpdateEmpData(formCollection["emp_id"], formCollection["emp_name"], formCollection["emp_email"], formCollection["emp_tel"], formCollection["emp_title"], wf.toi(status), formCollection["login_password"], Convert.ToInt32(formCollection["app_ser"]));


            // 3. Redirect ProClass
            return RedirectToAction("EmpList");
        }


        // 會員資料頁面
        public ActionResult MemberList()
        {
            return View();
        }

    }
}
