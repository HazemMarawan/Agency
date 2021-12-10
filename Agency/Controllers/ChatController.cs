using Agency.Auth;
using Agency.Helpers;
using Agency.Models;
using Agency.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Agency.Controllers
{
    [CustomAuthenticationFilter]
    public class ChatController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: Chat
        public ActionResult Index()
        {
            return View();
        }
    }
}