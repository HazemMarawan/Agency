using Agency.Auth;
using Agency.Models;
using Agency.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agency.Enums;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace Agency.Controllers
{
    [CustomAuthenticationFilter]
    public class MailController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: Mail
        public ActionResult Index()
        {
            User currentUser = Session["user"] as User;
           
            ViewBag.toUsers = db.Users.Where(s => s.id != currentUser.id).Select(s => new { s.id, s.full_name }).ToList();

            List<EmailViewModel> inboxMails = (from user in db.Users
                                               join email in db.Emails on user.id equals email.from_user
                                               select new EmailViewModel
                                               {
                                                   id = email.id,
                                                   subject = email.subject,
                                                   body = email.body,
                                                   stringCreatedAt = email.created_at.ToString(),
                                                   created_at = email.created_at,
                                                   stringFromUser = user.full_name,
                                                   to_user = email.to_user,
                                                   userImage = user.image,
                                                   emailAttachments = db.EmailAttachments.Where(e=>e.email_id == email.id).Select(e=>new EmailAttachmentViewModel
                                                   {
                                                       attachmentPath = e.attachmentPath
                                                   }).ToList()
                                               }).Where(s => s.to_user == currentUser.id).OrderByDescending(s => s.created_at).ToList();

            List<EmailViewModel> sendMails = (from user in db.Users
                                               join email in db.Emails on user.id equals email.to_user
                                               select new EmailViewModel
                                               {
                                                   id = user.id,
                                                   subject = email.subject,
                                                   body = email.body,
                                                   stringCreatedAt = email.created_at.ToString(),
                                                   created_at = email.created_at,
                                                   stringToUser = user.full_name,
                                                   to_user = email.to_user,
                                                   userImage = user.image,
                                                   from_user = email.from_user,
                                                   emailAttachments = db.EmailAttachments.Where(e => e.email_id == email.id).Select(e => new EmailAttachmentViewModel
                                                   {
                                                       attachmentPath = e.attachmentPath
                                                   }).ToList()
                                               }).Where(s => s.from_user == currentUser.id).OrderByDescending(s=>s.created_at).ToList();
            MailboxViewModel mailboxViewModel = new MailboxViewModel();
            mailboxViewModel.inboxMails = inboxMails;
            mailboxViewModel.sendMails = sendMails;

            ViewBag.currentUserName = currentUser.full_name;

            return View(mailboxViewModel);
        }

        [HttpPost]
        public JsonResult sendMail(EmailViewModel emailVM)
        {
            User currentUser = Session["user"] as User;

            Email email = AutoMapper.Mapper.Map<EmailViewModel, Email>(emailVM);

            email.from_user = currentUser.id;
            email.active = 1;
            email.updated_at = DateTime.Now;
            email.created_at = DateTime.Now;

            db.Emails.Add(email);
            db.SaveChanges();

            if(emailVM.attachments.Count() != 0)
            { 
                foreach(var file in emailVM.attachments)
                {
                    Guid guid = Guid.NewGuid();
                    var InputFileName = Path.GetFileName(file.FileName);
                    var ServerSavePath = Path.Combine(Server.MapPath("~/Email/Attachments/") + guid.ToString() + "attachment" + Path.GetExtension(file.FileName));
                    file.SaveAs(ServerSavePath);

                    EmailAttachment emailAttachment = new EmailAttachment();
                    emailAttachment.attachmentPath = "/Email/Attachments/" + guid.ToString() + "attachment" + Path.GetExtension(file.FileName);
                    emailAttachment.email_id = email.id;

                    db.EmailAttachments.Add(emailAttachment);
                    db.SaveChanges();
                }
            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }
    }
}