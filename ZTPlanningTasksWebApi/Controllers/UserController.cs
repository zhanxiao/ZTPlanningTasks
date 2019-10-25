using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ZTPlanningTasksWebApi.Controllers
{
    public class UserController : ApiController
    {
        [System.Web.Http.HttpPost]
        public IHttpActionResult Login()
        {            
            return Json(new { code = 20000, data = new { token = "admin-token" } });
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult Info()
        {
            return Json(new {
                code = 20000,
                data = new {
                    roles = new string[] { "admin" },
                    introduction = "I am a super administrator",
                    avatar = "https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif",
                    name = "Super Admin"
                }
            });
        }

        [System.Web.Http.HttpPut]
        public IHttpActionResult Logout()
        {
            return Json(new { code = 20000, data = "success" });
        }

    }
}
