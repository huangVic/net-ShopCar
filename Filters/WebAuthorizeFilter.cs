using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;


namespace ShopCar.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class WebAuthorizeFilter : ActionFilterAttribute
    {
        //表示是否檢查登錄
        public bool AuthFlag { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContextInfo fcinfo = new filterContextInfo(filterContext);

            Debug.WriteLine(" >>>> OnActionExecuting -------------------------->>>> ");
            Debug.WriteLine(" >>>> AuthFlag : " + AuthFlag);
            Debug.WriteLine(" >>>> controllerName : " + fcinfo.controllerName);
            Debug.WriteLine(" >>>> actionName : " + fcinfo.actionName);

            if (AuthFlag)
            {
                // 驗證是否登入
                if (filterContext.HttpContext.Session["emp_ser"] == null && filterContext.HttpContext.Session["emp_email"] == null)
                {
                    // 尚未登入
                    Debug.WriteLine(" >>>> 尚未登入");
                    filterContext.HttpContext.Response.Redirect("/Auth/Login");
                }
                else
                {
                    // 已登入
                    string url = filterContext.HttpContext.Request.RawUrl;
                    Debug.WriteLine(" >>>> 已登入 url: " + url);
                }
            }
            return;
        }
        /*
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 請確定一個應用程式啟動只起始一次 ASP.NET Simple Membership
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<UsersContext>(null);

                try
                {
                    using (var context = new UsersContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // 建立沒有 Entity Framework 移轉結構描述的 SimpleMembership 資料庫 
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("無法起始 ASP.NET Simple Membership 資料庫。如需詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
         * */
    }




    public class filterContextInfo
    {
        public filterContextInfo(ActionExecutingContext filterContext)
        {
            // ---------  #region 取得連接中的字串
            // 取得網域名()
            domainName = filterContext.HttpContext.Request.Url.Authority;

            // 取得模組名稱
            //  module = filterContext.HttpContext.Request.Url.Segments[1].Replace('/', ' ').Trim();

            // 取得 controller 名稱
            controllerName = filterContext.RouteData.Values["controller"].ToString();

            // 取得 action 名稱
            actionName = filterContext.RouteData.Values["action"].ToString();

            // --------- #endregion
        }

        // <summary>
        // 取得網域名
        // </summary>
        public string domainName { get; set; }
        // <summary>
        // 取得模組名稱
        // </summary>
        public string module { get; set; }
        // <summary>
        // 取得 controller 名稱
        // </summary>
        public string controllerName { get; set; }
        // <summary>
        // 取得 action 名稱
        // </summary>
        public string actionName { get; set; }

    }
}
