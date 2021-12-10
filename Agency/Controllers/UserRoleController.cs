using Agency.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agency.Controllers
{
    [CustomAuthenticationFilter]
    public class UserRoleController : Controller
    {
        // GET: UserRole
        public ActionResult Index()
        {
            return View();
        }
    }
}