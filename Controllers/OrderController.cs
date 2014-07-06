using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ShopCar.Filters;

namespace ShopCar.Controllers
{
    [WebAuthorizeFilter(AuthFlag = true)]
    public class OrderController : Controller
    {
        private ShopCar.Class.wfdb wf = new ShopCar.Class.wfdb();
        
        
        //
        // GET: /Order/

        public ActionResult OrderList()
        {
            return View();
        }

        public ActionResult OrderAllList()
        {
            //主檔
            ArrayList list = new ArrayList();
 

            Models.ShopCarDatasetTableAdapters.OrderTableAdapter orderMadp = new Models.ShopCarDatasetTableAdapters.OrderTableAdapter();
            DataTable dtMaster = orderMadp.GetData();

            
            string appstatus;

            Models.ShopCarDatasetTableAdapters.DataTable1TableAdapter orderDadp = new Models.ShopCarDatasetTableAdapters.DataTable1TableAdapter();
           

            foreach (DataRow drowM in dtMaster.Rows)
            {

                DataTable dtDetail = orderDadp.GetOneOrderData(wf.toi(drowM["app_ser"]));

                // 訂單明細
                ArrayList detailList = new ArrayList();
                
                foreach (DataRow drowD in dtDetail.Rows)
                {

                    if (wf.tos(drowD["app_dser"]) !="")
                    {
                        var detailItem = new
                        {
                            app_dser = wf.tos(drowD["app_dser"]),
                            pro_app_ser = wf.tos(drowD["pro_app_ser"]), // 產品app_Ser
                            productID = wf.tos(drowD["productID"]),
                            proName = wf.tos(drowD["proName"]),
                            prod_price = wf.toi(drowD["prod_price"]),   // 有特價([prod_special_price]) 已特價為主, 沒有在給原價資訊 目前先抓prod_price
                            pro_class_id = wf.tos(drowD["prod_class_id"]),     // 產品分類id
                            pro_class = wf.tos(drowD["pro_class_name"])  // 分類中文名稱
                        };
                        detailList.Add(detailItem);
                     }
                }

                if (wf.tos(drowM["app_status"])=="0")
                { appstatus = "取消"; }
                else if (wf.tos(drowM["app_status"]) == "10")
                { appstatus = "貨物退回"; }
                else if (wf.tos(drowM["app_status"]) == "100")
                { appstatus = "處裡中"; }
                else if (wf.tos(drowM["app_status"]) == "200")
                { appstatus = "送貨中"; }
                else if (wf.tos(drowM["app_status"]) == "900")
                { appstatus = "結案"; }
                else if (wf.tos(drowM["app_status"]) == "999")
                { appstatus = "取消訂單"; }
                else
                { appstatus = "無狀態"; }


                // 訂單主表單
                var orderItem = new
                {
                    app_ser = wf.tos(drowM["app_ser"]),
                    app_no = wf.tos(drowM["app_no"]),
                    create_date = ((DateTime)drowM["create_date"]).ToString("yyyy/MM/dd"),
                    amount = wf.tos(drowM["amount"]),
                    app_status = appstatus,   // 訂單狀態: 0:取消, 10:貨物退回, 100:處裡中, 200:送貨中, 900:結案, 999:取消訂單 
                    purchaser = wf.tos(drowM["purchaser"]),
                    purchaser_phone = wf.tos(drowM["purchaser_phone"]),
                    purchaser_addr = wf.tos(drowM["purchaser_addr"]),
                    details = detailList
                };

                list.Add(orderItem);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }



        // 新增訂單
        public ActionResult OrderAdd()
        {
            return View();
        }

        


        // 編輯訂單畫面
        public ActionResult OrderEdit(string app_ser)
        {
            var orderItem = new
            {
                app_ser = app_ser
            };
            return View("OrderEdit",orderItem);
        }

          // 訂單修改儲存
        [HttpPost]
        public ActionResult OrderUpdate(FormCollection formCollection)
        {

            // 1. 取得前端 form 的欄位資料
            System.Diagnostics.Debug.WriteLine(" >>>> OrderUpdate -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> OrderUpdate app_ser: " + formCollection["app_ser"]);
            System.Diagnostics.Debug.WriteLine(" >>>> OrderUpdate app_No: " + formCollection["app_No"]);
            System.Diagnostics.Debug.WriteLine(" >>>> OrderUpdate app_status: " + formCollection["app_status"]);
            System.Diagnostics.Debug.WriteLine(" >>>> OrderUpdate create_date: " + formCollection["create_date"]);
            System.Diagnostics.Debug.WriteLine(" >>>> OrderUpdate purchaser: " + formCollection["purchaser"]);
            System.Diagnostics.Debug.WriteLine(" >>>> OrderUpdate purchaser_phone: " + formCollection["purchaser_phone"]);
            System.Diagnostics.Debug.WriteLine(" >>>> OrderUpdate purchaser_addr: " + formCollection["purchaser_addr"]);
            System.Diagnostics.Debug.WriteLine(" >>>> OrderUpdate amount: " + formCollection["amount"]);

            //*********** 2. 更新 app_status / create_date / amount  (未開發) ***********


            // 3. 回傳app_Ser
            var orderItem = new
            {
                app_ser = formCollection["app_ser"]
            };
            return View("OrderEdit", orderItem);
        }

        // 取得單一訂單 API
        public ActionResult GetOrderItem(string app_ser)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> GetOrderItem 取得單一訂單 -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> GetOrderItem app_ser: " + app_ser);

            //取得訂單資料回傳前端
            ArrayList list = new ArrayList();


            Models.ShopCarDatasetTableAdapters.OrderTableAdapter orderMadp = new Models.ShopCarDatasetTableAdapters.OrderTableAdapter();
            DataTable dtMaster = orderMadp.GetOneOrderMasteData(wf.toi(app_ser));


            Models.ShopCarDatasetTableAdapters.DataTable1TableAdapter orderDadp = new Models.ShopCarDatasetTableAdapters.DataTable1TableAdapter();

            foreach (DataRow drowM in dtMaster.Rows)
            {
                 DataTable dtDetail = orderDadp.GetOneOrderData(wf.toi(drowM["app_ser"]));

                 // 訂單明細1
                 ArrayList detailList = new ArrayList();

                foreach (DataRow drowD in dtDetail.Rows)
                {

                    if(wf.tos(drowD["app_dser"]) != "")
                    { 
                        var detailItem = new
                        {
                            app_dser = wf.tos(drowD["app_dser"]),
                            pro_app_ser = wf.tos(drowD["pro_app_ser"]), // 產品app_Ser
                            productID = wf.tos(drowD["productID"]),
                            proName = wf.tos(drowD["proName"]),
                            prod_price = wf.toi(drowD["prod_price"]),  // 有特價([prod_special_price]) 已特價為主, 沒有在給原價資訊 目前先抓prod_price
                            pro_class_id = wf.tos(drowD["prod_class_id"]),     // 產品分類id
                            pro_class = wf.tos(drowD["pro_class_name"])  // 分類中文名稱
                        };
                        detailList.Add(detailItem);
                    }
                }


                //string appstatus;
                /*
                if (wf.tos(drowM["app_status"]) == "0")
                { appstatus = "取消"; }
                else if (wf.tos(drowM["app_status"]) == "10")
                { appstatus = "貨物退回"; }
                else if (wf.tos(drowM["app_status"]) == "100")
                { appstatus = "處裡中"; }
                else if (wf.tos(drowM["app_status"]) == "200")
                { appstatus = "送貨中"; }
                else if (wf.tos(drowM["app_status"]) == "900")
                { appstatus = "結案"; }
                else if (wf.tos(drowM["app_status"]) == "999")
                { appstatus = "取消訂單"; }
                else
                { appstatus = "無狀態"; }
                */

                // 訂單主表單
                var orderItem = new
                {
                    app_ser = wf.tos(drowM["app_ser"]),
                    app_no = wf.tos(drowM["app_no"]),
                    create_date = ((DateTime)drowM["create_date"]).ToString("yyyy/MM/dd"),
                    amount = wf.tos(drowM["amount"]),
                    app_status = wf.tos(drowM["app_status"]),   // 訂單狀態: 0:取消, 10:貨物退回, 100:處裡中, 200:送貨中, 900:結案, 999:取消訂單 
                    purchaser = wf.tos(drowM["purchaser"]),
                    purchaser_phone = wf.tos(drowM["purchaser_phone"]),
                    purchaser_addr = wf.tos(drowM["purchaser_addr"]),
                    details = detailList
                };

                list.Add(orderItem);
            }


            return Json(list, JsonRequestBehavior.AllowGet);
        }


        // 新增訂單明細
        [HttpPost]
        public ActionResult CreateOrderItem(FormCollection formCollection)
        {
            // 1. 取得前端 form 的欄位資料
            System.Diagnostics.Debug.WriteLine(" >>>> CreateOrderItem -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> CreateOrderItem order_app_ser: " + formCollection["order_app_ser"]);
            System.Diagnostics.Debug.WriteLine(" >>>> CreateOrderItem product_app_ser: " + formCollection["product_app_ser"]);

            //******** 2.訂單明細新增資料庫   (未開發) ***********


            var orderItem = new
            {
                app_ser = formCollection["order_app_ser"]
            };
            return View("OrderEdit", orderItem);
        }


        // 更新訂單明細
        [HttpPost]
        public ActionResult UpdateOrderItem(FormCollection formCollection)
        {
            // 1. 取得前端 form 的欄位資料
            System.Diagnostics.Debug.WriteLine(" >>>> UpdateOrderItem -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> UpdateOrderItem order_app_ser: " + formCollection["order_app_ser"]);
            System.Diagnostics.Debug.WriteLine(" >>>> UpdateOrderItem detail_app_dser: " + formCollection["detail_app_dser"]);  // 訂單明細 ser
            System.Diagnostics.Debug.WriteLine(" >>>> UpdateOrderItem product_app_ser: " + formCollection["product_app_ser"]);  // 更新產品 ser
            
            //******** 2.更新資料庫訂單明細   (未開發) ***********




            // 3. 回傳order_app_ser
            var orderItem = new
            {
                app_ser = formCollection["order_app_ser"]
            };
            return View("OrderEdit", orderItem);
        }


        // 刪除訂單明細
        [HttpPost]
        public ActionResult DeleteOrderItem(FormCollection formCollection)
        {
            // 1. 取得前端 form 的欄位資料
            System.Diagnostics.Debug.WriteLine(" >>>> DeleteOrderItem -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> DeleteOrderItem order_app_ser: " + formCollection["order_app_ser"]);
            System.Diagnostics.Debug.WriteLine(" >>>> DeleteOrderItem detail_app_dser: " + formCollection["detail_app_dser"]);


            //******** 2.刪除資料庫訂單明細  (未開發) ***********


            // 3. 回傳order_app_ser
            var orderItem = new
            {
                app_ser = formCollection["order_app_ser"]
            };
            return View("OrderEdit", orderItem);
        }
    }
}
