using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ZTPlanningTasksWebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if(HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.StatusCode = 200;
                HttpContext.Current.Response.SubStatusCode = 200;
                HttpContext.Current.Response.End();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            var res = HttpContext.Current.Response;
            //res.Headers.Add("Access-Control-Allow-Origin", "http://localhost:9527");
            //res.Headers.Add("Access-Control-Allow-Headers", "Content-Type, api_key, Authorization, x-requested-with");
            //res.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS");
            //res.Headers.Add("Access-Control-Allow-Credentials", "true");
            //res.Headers.Add("Cache-Control", "no-cache");
        }
    }
}
