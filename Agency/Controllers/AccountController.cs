using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agency.Models;
namespace Agency.Controllers
{
    public class AccountController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User user)
        {
            User currentUser = db.Users.Where(s => s.user_name.ToLower() == user.user_name.ToLower() && s.password == user.password).FirstOrDefault();
            if (currentUser != null)
            {
                Session["user_name"] = currentUser.user_name;
                Session["password"] = currentUser.password;
                Session["type"] = currentUser.type;
                Session["id"] = currentUser.id;
                Session["user"] = currentUser;

                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}