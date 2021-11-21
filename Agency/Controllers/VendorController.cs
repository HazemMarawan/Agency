using Agency.Auth;
using Agency.Helpers;
using Agency.Models;
using Agency.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agency.Controllers
{
    [CustomAuthenticationFilter]
    public class VendorController : Controller
    {
        // GET: City
        AgencyDbContext db = new AgencyDbContext();
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
                var vendors = (from vendor in db.Vendors
                                select new VendorViewModel
                                {
                                    id = vendor.id,
                                    name = vendor.name,
                                    code = vendor.code,
                                    active = vendor.active,
                                });

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    vendors = vendors.Where(m => m.name.ToLower().Contains(searchValue.ToLower()) || m.id.ToString().ToLower().Contains(searchValue.ToLower()) ||
                     m.code.ToLower().Contains(searchValue.ToLower()));
                }

                //total number of rows count     
                var displayResult = vendors.OrderByDescending(u => u.id).Skip(skip)
                     .Take(pageSize).ToList();
                var totalRecords = vendors.Count();

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
        public JsonResult saveVendor(VendorViewModel vendorVM)
        {

            if (vendorVM.id == 0)
            {
                Vendor vendor = AutoMapper.Mapper.Map<VendorViewModel, Vendor>(vendorVM);

                vendor.created_at = DateTime.Now;
                vendor.updated_at = DateTime.Now;
                vendor.created_by = Session["id"].ToString().ToInt();

                db.Vendors.Add(vendor);
                db.SaveChanges();
            }
            else
            {

                Vendor oldVendor = db.Vendors.Find(vendorVM.id);

                oldVendor.name = vendorVM.name;
                oldVendor.code = vendorVM.code;
                oldVendor.active = vendorVM.active;
                oldVendor.updated_by = Session["id"].ToString().ToInt();
                oldVendor.updated_at = DateTime.Now;

                db.Entry(oldVendor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult deleteVendor(int id)
        {
            Vendor deleteVendor = db.Vendors.Find(id);
            db.Vendors.Remove(deleteVendor);
            db.SaveChanges();

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }
    }
}