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
    public class CityController : Controller
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
                var userData = (from city in db.Cities
                                select new CityViewModel
                                {
                                    id = city.id,
                                    name = city.name,
                                    description = city.description,
                                    active = city.active,
                                    hotels = (from hotel in db.Hotels
                                              select new HotelViewModel
                                              {
                                                  id = hotel.id,
                                                  name = hotel.name,
                                                  city_id = hotel.city_id
                                              }).Where(s=>s.city_id == city.id).ToList()

                                });

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    userData = userData.Where(m => m.name.ToLower().Contains(searchValue.ToLower()) || m.id.ToString().ToLower().Contains(searchValue.ToLower()) ||
                     m.description.ToLower().Contains(searchValue.ToLower()));
                }

                //total number of rows count     
                var displayResult = userData.OrderByDescending(u => u.id).Skip(skip)
                     .Take(pageSize).ToList();
                var totalRecords = userData.Count();

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
        public JsonResult saveCity(CityViewModel cityVM)
        {

            if (cityVM.id == 0)
            {
                City city = AutoMapper.Mapper.Map<CityViewModel, City>(cityVM);

                city.created_at = DateTime.Now;
                city.updated_at = DateTime.Now;
                city.created_by = Session["id"].ToString().ToInt();

                db.Cities.Add(city);
                db.SaveChanges();
            }
            else
            {

                City oldCity = db.Cities.Find(cityVM.id);

                oldCity.name = cityVM.name;
                oldCity.description = cityVM.description;
                oldCity.active = cityVM.active;
                oldCity.updated_by = Session["id"].ToString().ToInt();
                oldCity.updated_at = DateTime.Now;

                db.Entry(oldCity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult deleteCity(int id)
        {
            City deleteCity = db.Cities.Find(id);
            db.Cities.Remove(deleteCity);
            db.SaveChanges();

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }
    }
}