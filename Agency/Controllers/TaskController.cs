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
    public class TaskController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: Task
        public ActionResult Index()
        {
            User currentUser = Session["user"] as User;

            UserTaskViewModel userTasks = new UserTaskViewModel();


            userTasks.allTasks = (from task in db.Tasks
                                  join user_task in db.UserTasks on task.id equals user_task.task_id
                                  join user in db.Users on user_task.user_id equals user.id
                                  select new TaskViewModel
                                  {
                                      id = task.id,
                                      user_task_id = user_task.id,
                                      title = task.title,
                                      description = task.description,
                                      stringCreatedAt = task.created_at.ToString(),
                                      status = user_task.status,
                                      created_at = task.created_at,
                                      created_by = task.created_by,
                                      stringCreatedToBy = user.full_name,

                                  }).Where(s => s.created_by == currentUser.id).OrderByDescending(s => s.created_at).ToList();

            userTasks.myTasks = (from task in db.Tasks
                             join user in db.Users on task.created_by equals user.id
                             join user_task in db.UserTasks on task.id equals user_task.task_id
                             select new TaskViewModel
                             {
                                 id = task.id,
                                 user_task_id = user_task.id,
                                 title = task.title,
                                 description = task.description,
                                 stringCreatedAt = task.created_at.ToString(),
                                 user_id = user_task.user_id,
                                 status = user_task.status,
                                 created_at = task.created_at,
                                 created_by = task.created_by,
                                 stringCreatedToBy = user.full_name

                             }).Where(s => s.user_id == currentUser.id).OrderByDescending(s => s.created_at).ToList();
            
           
                ViewBag.users = db.Users.Where(s =>s.id != currentUser.id).Select(s => new { s.id, s.full_name }).ToList();

            return View(userTasks);
        }

        [HttpPost]
        public JsonResult saveTask(TaskViewModel taskVM)
        {
            User currentUser = Session["user"] as User;
            Task task = AutoMapper.Mapper.Map<TaskViewModel, Task>(taskVM);

            task.created_by = currentUser.id;

            task.updated_at = DateTime.Now;
            task.created_at = DateTime.Now;

            db.Tasks.Add(task);
            db.SaveChanges();

            foreach (int user_id in taskVM.user_ids)
            {
                UserTask userTask = new UserTask();
                userTask.task_id = task.id;
                userTask.user_id = user_id;
                userTask.status = (int)UserTaskStatus.WatingForAction;

                db.UserTasks.Add(userTask);
                db.SaveChanges();
            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult followTask(int id)
        {
            UserTask userTask = db.UserTasks.Where(s => s.id == id).FirstOrDefault();
            userTask.status = (int)UserTaskStatus.Follow;

            db.Entry(userTask).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            User currentUser = Session["user"] as User;

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult doneTask(int id)
        {
            UserTask userTask = db.UserTasks.Where(s => s.id == id).FirstOrDefault();
            userTask.status = (int)UserTaskStatus.Done;

            db.Entry(userTask).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            User currentUser = Session["user"] as User;

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult deleteTask(int id)
        {
            UserTask userTask = db.UserTasks.Find(id);
            db.UserTasks.Remove(userTask);
            db.SaveChanges();

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }
    }
}