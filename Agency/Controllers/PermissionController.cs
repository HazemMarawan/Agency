using Newtonsoft.Json;
using Agency.Auth;
using Agency.Models;
using Agency.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agency.Controllers
{
    [CustomAuthenticationFilter]
    public class PermissionController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: Default
        public ActionResult Index()
        {
            if (!can.hasPermission("access_permission"))
                return RedirectToAction("Error404", "Error");

            if (Request.IsAjaxRequest())
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                var from_date = Request.Form.GetValues("columns[0][search][value]")[0];
                var to_date = Request.Form.GetValues("columns[1][search][value]")[0];
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                // Getting all data    
                var permissionData = (from permission in db.Permissions
                                      join permissiongroup in db.PermissionGroups on permission.permission_group_id equals permissiongroup.id

                                      select new PermissionViewModel
                                      {
                                          id = permission.id,
                                          name = permission.name,
                                          nice_name = permission.nice_name,
                                          description = permission.description,
                                          permission_group_id = permissiongroup.id,
                                          permission_group = permissiongroup.name
                                      });

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    permissionData = permissionData.Where(m => m.name.ToLower().Contains(searchValue.ToLower()) || m.id.ToString().ToLower().Contains(searchValue.ToLower()) ||
                     m.description.ToLower().Contains(searchValue.ToLower()) || m.nice_name.ToLower().Contains(searchValue.ToLower()));
                }

                //total number of rows count     
                var displayResult = permissionData.OrderByDescending(u => u.id).Skip(skip)
                     .Take(pageSize).ToList();
                var totalRecords = permissionData.Count();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = displayResult

                }, JsonRequestBehavior.AllowGet);

            }
            ViewBag.permissionGroups = db.PermissionGroups.Select(s => new { s.id, s.name }).ToList();
            return View();
        }
        [HttpPost]
        public JsonResult savePermission(PermissionViewModel PermissionVM)
        {
            User currentUser = Session["user"] as User;
            if (!can.hasPermission("edit_permission"))
            {
                return Json(new { message = "error" }, JsonRequestBehavior.AllowGet);
            }

            if (PermissionVM.id == 0)
            {

                Permission permission = AutoMapper.Mapper.Map<PermissionViewModel, Permission>(PermissionVM);

                permission.updated_at = DateTime.Now;
                permission.created_at = DateTime.Now;
                permission.active = 1;
                db.Permissions.Add(permission);
            }
            else
            {
                Permission oldPermission = db.Permissions.Find(PermissionVM.id);

                oldPermission.name = PermissionVM.name;
                oldPermission.description = PermissionVM.description;
                oldPermission.nice_name = PermissionVM.nice_name;
                oldPermission.updated_at = DateTime.Now;

                db.Entry(oldPermission).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult deletePermission(int id)
        {
            User currentUser = Session["user"] as User;
            if (!can.hasPermission("delete_permission"))
            {
                return Json(new { message = "error" }, JsonRequestBehavior.AllowGet);
            }


            Permission deletedPermission = db.Permissions.Find(id);
            deletedPermission.active = 0;
            db.Entry(deletedPermission).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }
    }
}