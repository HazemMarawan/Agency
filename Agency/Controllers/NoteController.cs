using Agency.Auth;
using Agency.Models;
using Agency.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agency.Enums;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace Agency.Controllers
{
    [CustomAuthenticationFilter]
    public class NoteController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: Note
        public ActionResult Index()
        {
            User currentUser = Session["user"] as User;
            List<NoteViewModel> notes = db.Notes.Where(s => s.created_by == currentUser.id).Select(s => new NoteViewModel
            {
                id = s.id,
                title = s.title,
                description = s.description,
                isFavourite = s.isFavourite,
                stringCreatedAt = s.created_at.ToString(),
                created_by = s.created_by,
                created_at = s.created_at
            }).Where(s=>s.created_by == currentUser.id).OrderByDescending(s=>s.created_at).ToList();
            return View(notes);
        }

        public JsonResult saveNote(NoteViewModel noteVM)
        {
            User currentUser = Session["user"] as User;

            Note note = AutoMapper.Mapper.Map<NoteViewModel, Note>(noteVM);

            note.created_by = currentUser.id;
            note.active = 1;
            note.updated_at = DateTime.Now;
            note.created_at = DateTime.Now;

            db.Notes.Add(note);
            db.SaveChanges();

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult setFavouriteNote(int id)
        {
            User currentUser = Session["user"] as User;

            Note note = db.Notes.Find(id);
            note.isFavourite = !note.isFavourite;
            db.Entry(note).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult deleteNote(int id)
        {
            Note note = db.Notes.Find(id);
            db.Notes.Remove(note);
            //note.active = 0;
            //db.Entry(note).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }
    }
}