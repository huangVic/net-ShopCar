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
    public class ProductController : Controller
    {
        private ShopCar.Class.wfdb wf = new ShopCar.Class.wfdb();
        private string _connectionString = ConfigurationManager.ConnectionStrings["ShopCarConnectionString"].ConnectionString;

        private string StorageRoot
        {
            get { return Path.Combine(Server.MapPath("~/Files")); }
        }

        public ActionResult ProToJsonData()
        {
            // 1. 資料庫連線 
            SqlConnection cn = new SqlConnection(_connectionString);

            // 2. SQL 指令 
            string sql = "select a.app_ser,a.ProductID as prod_no,a.ProName as prod_name,a.pro_desc as prod_desc, a.prod_price,a.prod_special_price,a.prod_feature,isnull(b.pro_class_name,'') as prod_class, a.pro_active from product a left join Product_Class b on a.prod_class_id=b.app_ser ";
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
        }

        // 取得所有被啟用的產品列表 Api
        public ActionResult ProToActiveProductData()
        {
            // 1. 資料庫連線 
            SqlConnection cn = new SqlConnection(_connectionString);

            // 2. SQL 指令 
            string sql = "select a.app_ser,a.ProductID as prod_no,a.ProName as prod_name,a.pro_desc as prod_desc, a.prod_price,a.prod_special_price,a.prod_feature,isnull(b.pro_class_name,'') as prod_class, a.pro_active from product a left join Product_Class b on a.prod_class_id=b.app_ser where a.pro_active='on' ";
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
                myHT.Add("productList", myDataTable);
                string obj_json = JsonConvert.SerializeObject(myHT);
                return Content(obj_json, "application/json");

            }
        }


        // 產品清單
        public ActionResult ProList()
        {
            return View();
        }

        // 產品新增畫面
        public ActionResult ProAdd()
        {
            return View();
        }


        // 產品新增儲存
        [HttpPost]
        public ActionResult ProCreate(FormCollection formCollection)
        {

            // 1. 取得前端 form 的欄位資料
            System.Diagnostics.Debug.WriteLine(" >>>> ProAdd -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> ProAdd proNo: " + formCollection["proNo"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProAdd proName: " + formCollection["proName"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProAdd proPrice: " + formCollection["proPrice"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProAdd proSpecialPrice: " + formCollection["proSpecialPrice"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProAdd proClassId: " + formCollection["proClassId"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProAdd pro_active: " + formCollection["pro_active"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProAdd prodFeature: " + formCollection["prodFeature"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProAdd prodDesc: " + formCollection["prodDesc"]);

            // 2. 寫入產品資料表
            //----> 程式碼 
            Models.ShopCarDatasetTableAdapters.ProductTableAdapter proadp = new Models.ShopCarDatasetTableAdapters.ProductTableAdapter();
            int ret_app_ser;
            decimal price;
            Nullable<decimal> special_price;
            price = Convert.ToDecimal(formCollection["proPrice"]);
            if (formCollection["proSpecialPrice"] == "")
            {
                special_price = null;
            }
            else { special_price = Convert.ToDecimal(formCollection["proSpecialPrice"]); }

            string proactive = "";
            if (!string.IsNullOrEmpty(formCollection["pro_active"]))
            {
                proactive = formCollection["pro_active"].ToString();
            }


            //ret_app_ser = Convert.ToInt32(proadp.InsertProductData(formCollection["proNo"], formCollection["proName"],price,special_price,formCollection["proClassId"],proactive,formCollection["prodFeature"],formCollection["prodDesc"]));

            ret_app_ser = Convert.ToInt32(proadp.InsertProductData(formCollection["proNo"], formCollection["proName"], price, special_price, Convert.ToInt32(formCollection["proClassId"]), proactive, formCollection["prodFeature"], formCollection["prodDesc"]));


            // 3. 使用物件 儲存起來回丟給前端 ProEdit.cshtml  


            var prodItem = new
            {
                appSer = ret_app_ser,
                proNo = formCollection["proNo"].ToString(),
                proName = formCollection["proName"].ToString(),
                proPrice = formCollection["proPrice"].ToString(),
                proSpecialPrice = formCollection["proSpecialPrice"].ToString(),
                proClassId = formCollection["proClassId"].ToString(),
                proActive = wf.tos(proactive),
                prodFeature = formCollection["prodFeature"].ToString(),
                prodDesc = formCollection["prodDesc"].ToString()
            };


            return View("ProEdit", prodItem);
        }

        public ActionResult ProIDCheck(string proid)
        {
            string msg; 

            // 1. 資料庫連線 
            SqlConnection cn = new SqlConnection(_connectionString);

            // 2. SQL 指令 
            string sql = "select count(*) from product where ProductID=@id";

            cn.Open();
            SqlCommand comm = new SqlCommand(sql, cn);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("@id", proid);
            if ((int)comm.ExecuteScalar() > 0)
            {
               msg = "Duplicate Key";
            }
            else {
               msg = "OK";                           
            }

            cn.Close();
            cn.Dispose();

            var result = new
            {
                message = msg
            };
            return Json(result, JsonRequestBehavior.AllowGet);
            //string obj_json = JsonConvert.SerializeObject(result);
            //return Content(obj_json, "application/json");
        
        }

        public ActionResult ClassExistProduct(int class_appser)
        {
            string msg;

            // 1. 資料庫連線 
            SqlConnection cn = new SqlConnection(_connectionString);

            // 2. SQL 指令 
            string sql = "select COUNT(*) from product_class a join Product b on a.app_ser=b.prod_class_id where a.app_ser=@id";

            cn.Open();
            SqlCommand comm = new SqlCommand(sql, cn);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("@id", class_appser);
            if ((int)comm.ExecuteScalar() > 0)
            {
                msg = "Exist";
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
            //string obj_json = JsonConvert.SerializeObject(result);
            //return Content(obj_json, "application/json");

        }



        // 產品修改畫面
        public ActionResult ProEdit(string appSer)
        {
            // 1. 取得Get 參數, app_Ser
            System.Diagnostics.Debug.WriteLine(" >>>> ProEdit AppSer: " + appSer);

            // 2. 於資料庫搜尋此筆資料
            //----> 程式碼 
            Models.ShopCarDatasetTableAdapters.ProductTableAdapter proadp = new Models.ShopCarDatasetTableAdapters.ProductTableAdapter();
            DataTable dt;
            dt = proadp.GetProductData(Convert.ToInt32(appSer));
            DataRow drow = dt.Rows[0];


            string price, s_price;

            price = wf.tos(drow["prod_price"]);
            if (price.IndexOf(".") >= 0)
            { price = price.Substring(0, price.IndexOf(".")); }


            s_price = wf.tos(drow["prod_special_price"]);
            if (s_price.IndexOf(".") >= 0)
            { s_price = s_price.Substring(0, s_price.IndexOf(".")); }


            /*
            if (wf.tos(drow["pro_active"]) == "on")
            {
                productOn = "on";
                productOff = "";
            }
            else
            {
                productOn = "";
                productOff = "on";
            }
             * */


            // 3. 將找到的Row, 使用物件 儲存起來回丟給前端 ProEdit.cshtml  
            var prodItem = new
            {
                appSer = wf.tos(drow["app_ser"]),
                proNo = wf.tos(drow["ProductID"]),
                proName = wf.tos(drow["ProName"]),
                proPrice = price,
                proSpecialPrice = s_price,
                proClassId = wf.tos(drow["prod_class_id"]),
                proActive = wf.tos(drow["pro_active"]),
                prodFeature = wf.tos(drow["prod_feature"]),
                prodDesc = wf.tos(drow["pro_desc"])
            };


            return View("ProEdit", prodItem);
        }

        // 產品修改儲存
        [HttpPost]
        public ActionResult ProUpdate(FormCollection formCollection)
        {

            // 1. 取得前端 form 的欄位資料
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdate -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdate proNo: " + formCollection["proNo"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdate proName: " + formCollection["proName"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdate proPrice: " + formCollection["proPrice"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdate proSpecialPrice: " + formCollection["proSpecialPrice"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdate proClassId: " + formCollection["proClassId"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdate pro_active: " + formCollection["pro_active"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdate prodFeature: " + formCollection["prodFeature"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdate prodDesc: " + formCollection["prodDesc"]);

            // 2. 更新產品資料表
            //----> 程式碼 
            decimal price;
            Nullable<decimal> special_price;
            price = Convert.ToDecimal(formCollection["proPrice"]);
            if (formCollection["proSpecialPrice"] == "")
            {
                special_price = null;
            }
            else { special_price = Convert.ToDecimal(formCollection["proSpecialPrice"]); }

            string proactive = "";
            if (!string.IsNullOrEmpty(formCollection["pro_active"]))
            {
                proactive = formCollection["pro_active"].ToString();
            }


            Models.ShopCarDatasetTableAdapters.ProductTableAdapter proadp = new Models.ShopCarDatasetTableAdapters.ProductTableAdapter();
            proadp.UpdateProductData(formCollection["proNo"], formCollection["proName"], price, special_price, Convert.ToInt32(formCollection["proClassId"]), proactive, formCollection["prodFeature"], formCollection["prodDesc"], Convert.ToInt32(formCollection["appser"]));


            // 3. 使用物件 儲存起來回丟給前端 ProEdit.cshtml  
            //string proActiveOnstr = "";
            //if (!string.IsNullOrEmpty(formCollection["proActiveOn"]))
            //{
            //    proActiveOnstr = formCollection["proActiveOn"].ToString();
            //}

            //string proActiveOffstr = "";
            //if (!string.IsNullOrEmpty(formCollection["proActiveOff"]))
            //{
            //    proActiveOffstr = formCollection["proActiveOff"].ToString();
            //}

            var prodItem = new
            {
                appSer = formCollection["appser"],
                proNo = formCollection["proNo"].ToString(),
                proName = formCollection["proName"].ToString(),
                proPrice = formCollection["proPrice"].ToString(),
                proSpecialPrice = formCollection["proSpecialPrice"].ToString(),
                proClassId = formCollection["proClassId"].ToString(),
                proActive = wf.tos(proactive),
                prodFeature = formCollection["prodFeature"].ToString(),
                prodDesc = formCollection["prodDesc"].ToString()
            };


            return View("ProEdit", prodItem);
        }

        // 刪除產品主檔
        [HttpPost]
        public ActionResult ProDelete(FormCollection formCollection)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> ProDelete -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> ProAdd proNo: " + formCollection["appSer"]);

            Int32 appser = Convert.ToInt32(formCollection["appSer"]);
            // 2. 刪除該appser產品
            //----> 程式碼 
            Models.ShopCarDatasetTableAdapters.ProductTableAdapter proadp = new Models.ShopCarDatasetTableAdapters.ProductTableAdapter();
            proadp.DeleteProductData(appser);


            // 3. Return pro_list  
            return RedirectToAction("ProList");



            //DataTable table = proadp.GetData();
            //Hashtable myHT = new Hashtable();
            //myHT.Add("aaData", table);

            //string obj_json = JsonConvert.SerializeObject(myHT);
            //return Content(obj_json, "application/json");
            //return RedirectToAction("ProList");
        }


        // 刪除檔案
        [HttpGet]
        public ActionResult Delete(string id)
        {
            string appdSer = id;  // 取得前端傳來要刪除某一筆file的 app_ser
            System.Diagnostics.Debug.WriteLine("[Get] Delete AppSer >>> " + appdSer);

            // 1. 先撈資料庫 File Table 的資料 (SQL)
            //----> 程式碼 
            Models.ShopCarDatasetTableAdapters.Product_PhotoTableAdapter photoadp = new Models.ShopCarDatasetTableAdapters.Product_PhotoTableAdapter();
            DataTable dt = photoadp.GetOnePhotoData(Convert.ToInt32(appdSer));


            // 2. 取得DataSet中的 DataTabe資訊
            //----> 程式碼

            // 3. 刪除一筆photo record
            //----> 程式碼 
            photoadp.DeleteOnePhotoData(Convert.ToInt32(appdSer));

            // 4. 刪除實體路徑檔案
            var filename =wf.tos(dt.Rows[0]["file_name"]);  // 透過資料表找到file Name
            var filePath = Path.Combine(Server.MapPath("~/Files"), filename);

            ArrayList list = new ArrayList();
            var obj = new DeleteMesg();

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);

                obj.sussess = "true";
                obj.message = "delete file " + id + " finish..";
            }
            else
            {

                obj.sussess = "false";
                obj.message = "delete file " + id + " finish..";
            }
            list.Add(obj);

            // 5. 回傳json 資訊
            Hashtable myHT = new Hashtable();
            myHT.Add("file", list);

            return Json(myHT, JsonRequestBehavior.AllowGet);
        }

        // 下載檔案
        [HttpGet]
        public void Download(string id)
        {
            var filename = id;
            var filePath = Path.Combine(Server.MapPath("~/Files"), filename);

            var context = HttpContext;

            if (System.IO.File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }


        // 修改 ActioneName 此 http 路徑為: http://loaclhost/home/UploadFiles ; 不是 http://loaclhost/home/GetFileList
        // 前端載入畫面時, 會先來這個 action 取得目前資料庫中的檔案清單
        [HttpGet, ActionName("UploadFiles")]
        public ActionResult GetFileList(string appSer)
        {
            System.Diagnostics.Debug.WriteLine("[Get] UploadFiles AppSer >>> " + appSer);
            ArrayList list = new ArrayList();


            // 1. 先撈資料庫 File Table 的資料 (SQL)
            //----> 程式碼 
            Models.ShopCarDatasetTableAdapters.Product_PhotoTableAdapter photoadp = new Models.ShopCarDatasetTableAdapters.Product_PhotoTableAdapter();
            DataTable dt = photoadp.GetPhotoList(Convert.ToInt32(appSer));

            // 2. 取得DataSet中的 DataTabe資訊
            //----> 程式碼

            // 3. 使用迴圈將每一筆 row 取出
            foreach (DataRow drow in dt.Rows)
            {
            //  ----> 新增每一個 statuses 將取出的row 資料塞入

            var statuses = new ViewDataUploadFilesResult();
            statuses.name = wf.tos(drow["file_name"]);
            statuses.size =wf.toi(drow["file_size"]);
            statuses.contentType = wf.tos(drow["content_type"]);
            statuses.url = wf.tos(drow["download_url"]);
            statuses.deleteUrl = wf.tos(drow["delete_url"]); //"/Product/Delete?id=" + appSer; // file table的 App_Ser
            statuses.thumbnailUrl = @"data:image/png;base64," + EncodeFile(wf.tos(drow["thumbnail_url"]));
            statuses.deleteType = wf.tos(drow["delete_type"]);
            list.Add(statuses);


            //var statuses2 = new ViewDataUploadFilesResult();
            //statuses2.name = "09.jpg";
            //statuses2.size = 123456;
            //statuses2.contentType = "image/jpeg";
            //statuses2.url = "/Files/09.jpg";
            //statuses2.deleteUrl = "/Product/Delete?id=" + "123455"; // file table的 App_Ser
            //statuses2.thumbnailUrl = "/Files/09.jpg";
            //statuses2.deleteType = "GET";
            //list.Add(statuses2);


            }

            // 將 arrayList 掛到 files 底下, 並轉成json回傳前端
            Hashtable myHT = new Hashtable();
            myHT.Add("files", list);

            return Json(myHT, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public ActionResult UploadFiles(FormCollection formCollection)
        {

            // 1. 接收前端傳入的資料
            System.Diagnostics.Debug.WriteLine("[Post] appSer >>> " + formCollection["appSer"]);

            int appser;
            appser = Convert.ToInt32(formCollection["appSer"]);

            // 2. 由appSer 找到 Master產品資料
            //----> 程式碼 
            


            // 3. 接下來做檔案上傳動作
            ArrayList list = new ArrayList();
            foreach (string file in Request.Files)
            {
                var statuses = new ViewDataUploadFilesResult();
                var headers = Request.Headers;


                // 4. 檔案上傳到伺服器
                if (string.IsNullOrEmpty(headers["X-File-Name"]))
                {
                    UploadWholeFile(Request, statuses, appser); //於此function 寫入File 資料表, 紀錄儲存路徑/檔案大小, 檔案名稱
                }
                else
                {
                    UploadPartialFile(headers["X-File-Name"], Request, statuses, appser);//於此function 寫入File 資料表, 紀錄儲存路徑/檔案大小, 檔案名稱
                }
                list.Add(statuses);
            }

            //Response.End();

            // 6. 回傳Json資料；將list 掛在files
            Hashtable myHT = new Hashtable();
            myHT.Add("files", list);


            /*
            JsonResult result = Json(statuses);
            result.ContentType = "text/json";
            */

            //string obj_json = JsonConvert.SerializeObject(myHT);
            //return Content(obj_json, "application/json");

            return Json(myHT);
        }

        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }


        private void UploadPartialFile(string fileName, HttpRequestBase request, ViewDataUploadFilesResult statuses,int appser)
        {
            if (request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];
            var inputStream = file.InputStream;

            var fullName = Path.Combine(StorageRoot, Path.GetFileName(fileName));

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }

            Models.ShopCarDatasetTableAdapters.Product_PhotoTableAdapter photoadp = new Models.ShopCarDatasetTableAdapters.Product_PhotoTableAdapter();
            statuses.name = fileName;
            statuses.size = file.ContentLength;
            statuses.contentType = file.ContentType;
            statuses.url = "/Files/" + fileName;
            statuses.deleteUrl = ""; //"/Product/Delete?id=" + fileName;
            statuses.thumbnailUrl = @"data:image/png;base64," + EncodeFile(fullName);
            statuses.deleteType = "GET";
            int ret_app_dser;
            ret_app_dser = Convert.ToInt32(photoadp.InsertPhoto(appser, statuses.name, statuses.size, statuses.url, fullName, statuses.deleteUrl, statuses.deleteType, statuses.contentType));

            statuses.deleteUrl = "/Product/Delete?id=" + Convert.ToString(ret_app_dser);
            photoadp.UpdateDeleteURL(statuses.deleteUrl, ret_app_dser);
            

            /*
            statuses.Add(new ViewDataUploadFilesResult()
            {
                name = fileName,
                size = file.ContentLength,
                type = file.ContentType,
                url = "/Home/Download/" + fileName,
                delete_url = "/Home/Delete/" + fileName,
                thumbnail_url = @"data:image/png;base64," + EncodeFile(fullName),
                delete_type = "GET",
            });
             * */

        }

        private void UploadWholeFile(HttpRequestBase request, ViewDataUploadFilesResult statuses,int appser)
        {
            for (int i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];

                var fullPath = Path.Combine(StorageRoot, Path.GetFileName(file.FileName));
                              
                file.SaveAs(fullPath);

                Models.ShopCarDatasetTableAdapters.Product_PhotoTableAdapter photoadp = new Models.ShopCarDatasetTableAdapters.Product_PhotoTableAdapter();
                statuses.name = file.FileName;
                statuses.size = file.ContentLength;
                statuses.contentType = file.ContentType;
                statuses.url = "/Files/" + file.FileName;
                statuses.deleteUrl = "";//"/Product/Delete?id=" + file.FileName;
                statuses.thumbnailUrl = @"data:image/png;base64," + EncodeFile(fullPath);
                statuses.deleteType = "GET";
                int ret_app_dser;
                ret_app_dser = Convert.ToInt32(photoadp.InsertPhoto(appser, statuses.name, statuses.size, statuses.url, fullPath, statuses.deleteUrl, statuses.deleteType, statuses.contentType));

                statuses.deleteUrl = "/Product/Delete?id=" + Convert.ToString(ret_app_dser);
                photoadp.UpdateDeleteURL(statuses.deleteUrl, ret_app_dser);

               /*
                statuses.Add(new ViewDataUploadFilesResult()
                {
                    name = file.FileName,
                    size = file.ContentLength,
                    type = file.ContentType,
                    url = "/Home/Download/" + file.FileName,
                    delete_url = "/Home/Delete/" + file.FileName,
                    thumbnail_url = @"data:image/png;base64," + EncodeFile(fullPath),
                    delete_type = "GET",
                });
                 * */
            }
        }


        // 產品分類畫面
        public ActionResult ProClass()
        {

            return View();
        }

        // 新增產品分類
        [HttpPost]
        public ActionResult ProCreateClass(FormCollection formCollection)
        {
            // 1. 取得formCollection資訊
            // 1. 取得前端 form 的欄位資料
            System.Diagnostics.Debug.WriteLine(" >>>> ProCreateClass -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> ProCreateClass clasee_No: " + formCollection["pro_class_no"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProCreateClass clasee_Name: " + formCollection["pro_class_name"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProCreateClass clasee_desc: " + formCollection["pro_class_desc"]);

            // 因為是新增calss, 故將class_active預設為on 
            Models.ShopCarDatasetTableAdapters.Product_ClassTableAdapter classadp = new Models.ShopCarDatasetTableAdapters.Product_ClassTableAdapter();


            // 2. 將欄位存到資料庫
            classadp.InsertProClass(formCollection["pro_class_no"], formCollection["pro_class_name"], formCollection["pro_class_desc"], "on");

            // 3. Redirect ProClass
            return RedirectToAction("ProClass");
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


        // 取得單一產品分類資料
        public ActionResult GetProClassItem(string app_ser)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> GetProClassItem -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> app_serxxx : " + app_ser);

            Models.ShopCarDatasetTableAdapters.Product_ClassTableAdapter classadp = new Models.ShopCarDatasetTableAdapters.Product_ClassTableAdapter();
            DataTable dt = classadp.GetOneClassData(Convert.ToInt32(app_ser));

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

                System.Diagnostics.Debug.WriteLine(" >>>> class name: : " + prodClass.pro_class_name);    
                list.Add(prodClass);

            }

            
            

            Hashtable myHT = new Hashtable();
            myHT.Add("classItem", list);

            
            //return Json(myHT);
            return Json(myHT, JsonRequestBehavior.AllowGet);

        }



        // 更新產品分類
        [HttpPost]
        public ActionResult ProUpdateClass(FormCollection formCollection)
        {
            // 1. 取得formCollection資訊
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdateClass -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdateClass app_ser: " + formCollection["app_ser"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdateClass clasee_No: " + formCollection["pro_class_no"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdateClass clasee_Name: " + formCollection["pro_class_name"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdateClass clasee_desc: " + formCollection["pro_class_desc"]);
            System.Diagnostics.Debug.WriteLine(" >>>> ProUpdateClass class_active: " + formCollection["class_active"]);

            Models.ShopCarDatasetTableAdapters.Product_ClassTableAdapter classadp = new Models.ShopCarDatasetTableAdapters.Product_ClassTableAdapter();


            // 2. 將欄位存到資料庫
            classadp.UpdateClassData(formCollection["pro_class_no"], formCollection["pro_class_name"], formCollection["pro_class_desc"], formCollection["class_active"], Convert.ToInt32(formCollection["app_ser"]));

            // 3. Redirect ProClass
            return RedirectToAction("ProClass");
        }


        // 刪除產品分類
        [HttpPost]
        public ActionResult ProClasDelete(FormCollection formCollection)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> ProClasDelete -------------------------->>>> ");
            System.Diagnostics.Debug.WriteLine(" >>>> ProClasDelete app_ser: " + formCollection["app_ser"]);

            Int32 appser = Convert.ToInt32(formCollection["app_ser"]);
            // 1. 刪除該appser產品
            //----> 程式碼 

            Models.ShopCarDatasetTableAdapters.Product_ClassTableAdapter classadp = new Models.ShopCarDatasetTableAdapters.Product_ClassTableAdapter();

            classadp.DeleteClassData(appser);

            // 2. Return pro_list  
            return RedirectToAction("ProClass");
        }

    }

    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public int size { get; set; }
        public string contentType { get; set; }
        public string url { get; set; }
        public string deleteUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteType { get; set; }
    }

    public class DeleteMesg
    {
        public string sussess { get; set; }
        public string message { get; set; }
    }

    public class ProductItem
    {
       public string  proNo { get; set; }
       public string  proName  { get; set; }
       public string  proPrice { get; set; }
       public string  proSpecialPrice { get; set; }
       public string  proClassId { get; set; }
       public string  proActive { get; set; }
       public string  prodFeature { get; set; }
       public string  prodDesc { get; set; }
    }
}
