using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZTPlanningTasksWebApi
{
    public class SpecialMethodModule : IHttpModule
    {
        public SpecialMethodModule() { }

        public void Dispose() { }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            HttpContext context = app.Context;
            if(context.Request.HttpMethod.ToUpper() == "OPTIONS")
            {
                context.Response.StatusCode = 200;
                context.Response.End();
            }
        }
    }
}