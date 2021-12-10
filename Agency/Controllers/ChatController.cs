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
            User currentUser = Session["user"] as User;
            List<UserViewModel> users = (from user in db.Users
                                         select new UserViewModel
                                         {
                                             id = user.id,
                                             full_name = user.full_name,
                                             imagePath = user.image,
                                             chats = db.Chats.Where(c => c.from_user == currentUser.id || c.to_user == currentUser.id).Select(c => new ChatViewModel {
                                                 id = c.id,
                                                 message = c.message,
                                                 message_class = c.from_user == currentUser.id? "bubble you": "bubble me",
                                                 string_created_at = c.created_at.ToString(),
                                                 created_at = c.created_at
                                             }).OrderBy(c=>c.created_at).ToList()
                                         }).Where(u=>u.id != currentUser.id).ToList();
                                         
            return View(users);
        }

        public JsonResult SaveMessage(ChatViewModel chatViewModel)
        {
            User currentUser = Session["user"] as User;

            Chat chat = AutoMapper.Mapper.Map<ChatViewModel, Chat>(chatViewModel);
            chat.from_user = currentUser.id;
            chat.created_at = DateTime.Now;
            db.Chats.Add(chat);
            db.SaveChanges();
            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }
    }
}