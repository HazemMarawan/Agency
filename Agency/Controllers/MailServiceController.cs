using Agency.Auth;
using Agency.Helpers;
using Agency.Models;
using Agency.Services;
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
    public class MailServiceController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: MailService
        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                // Getting all data    
                var mailservers = (from mailserver in db.MailServers
                               select new MailServerViewModel
                               {
                                   id = mailserver.id,
                                   outgoing_mail = mailserver.outgoing_mail,
                                   outgoing_mail_password = mailserver.outgoing_mail_password,
                                   outgoing_mail_server = mailserver.outgoing_mail_server,
                                   port = mailserver.port,
                                   title = mailserver.title,
                                   welcome_message = mailserver.welcome_message,
                                   incoming_mail = mailserver.incoming_mail,
                                   type = mailserver.type
                               });

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    mailservers = mailservers.Where(m => m.outgoing_mail.ToLower().Contains(searchValue.ToLower())
                    || m.id.ToString().ToLower().Contains(searchValue.ToLower())
                    || m.outgoing_mail_password.ToLower().Contains(searchValue.ToLower())
                    || m.outgoing_mail_server.ToLower().Contains(searchValue.ToLower())
                    || m.port.ToLower().Contains(searchValue.ToLower())
                    || m.title.ToLower().Contains(searchValue.ToLower())
                    || m.welcome_message.ToLower().Contains(searchValue.ToLower())
                    || m.incoming_mail.ToLower().Contains(searchValue.ToLower())
                    );
                }

                //total number of rows count     
                var displayResult = mailservers.OrderByDescending(u => u.id).Skip(skip)
                     .Take(pageSize).ToList();
                var totalRecords = mailservers.Count();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = displayResult

                }, JsonRequestBehavior.AllowGet);

            }

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult saveMailServer(MailServerViewModel mailServerViewModel)
        {
            MailServer oldMailServer = db.MailServers.Find(mailServerViewModel.id);

            oldMailServer.outgoing_mail = mailServerViewModel.outgoing_mail;
            oldMailServer.outgoing_mail_password = mailServerViewModel.outgoing_mail_password;
            oldMailServer.outgoing_mail_server = mailServerViewModel.outgoing_mail_server;
            oldMailServer.port = mailServerViewModel.port;
            oldMailServer.title = mailServerViewModel.title;
            oldMailServer.welcome_message = mailServerViewModel.welcome_message;
            oldMailServer.incoming_mail = mailServerViewModel.incoming_mail;
            //oldMailServer.type = mailServerViewModel.type;
            oldMailServer.updated_by = Session["id"].ToString().ToInt();
            oldMailServer.updated_at = DateTime.Now;

            db.SaveChanges();

            return RedirectToAction("Index");

        }
    }
}