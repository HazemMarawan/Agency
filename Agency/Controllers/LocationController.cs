using Agency.Auth;
using Agency.Enums;
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
    public class LocationController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: Location
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
                var userData = (from location in db.Locations
                                join locationCity in db.Cities on location.city_id equals locationCity.id
                                select new LocationViewModel
                                {
                                    id = location.id,
                                    name = location.name,
                                    description = location.description,
                                    active = location.active,
                                    city_id = location.city_id,
                                    location_city_name = locationCity.name,
                                    HotelLocations = (from hotelLocation in db.HotelLocations
                                                      join hotel in db.Hotels on hotelLocation.hotel_id equals hotel.id
                                                      join hotelCity in db.Cities on hotel.city_id equals hotelCity.id
                                                      select new HotelLocationViewModel
                                                      {
                                                          id = hotelLocation.id,
                                                          distance = hotelLocation.distance,
                                                          map_link = hotelLocation.map_link,
                                                          hotel_id = hotelLocation.hotel_id,
                                                          location_id = hotelLocation.location_id,
                                                          hotel_name = hotel.name,
                                                          hotel_rate = hotel.rate,
                                                          hotel_city_id = hotelCity.id,
                                                          hotel_city_name = hotelCity.name,
                                                            
                                                      }).Where(h => h.location_id == location.id).ToList()

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
            ViewBag.Hotels = db.Hotels.Select(s => new { s.id, s.name }).ToList();
            ViewBag.Cities = db.Cities.Select(s => new { s.id, s.name }).ToList();

            return View();
        }

        public ActionResult saveLocation(LocationViewModel locationVM)
        {
            if (locationVM.id == 0)
            {
                Location location = AutoMapper.Mapper.Map<LocationViewModel, Location>(locationVM);

                location.created_at = DateTime.Now;
                location.created_by = Session["id"].ToString().ToInt();
                db.Locations.Add(location);
                db.SaveChanges();
            }
            else
            {
                Location loc = db.Locations.Find(locationVM.id);

                loc.name = locationVM.distance;
                loc.description = locationVM.description;
                loc.active = locationVM.active;
                loc.city_id = locationVM.city_id;

                loc.updated_at = DateTime.Now;
                loc.updated_by = Session["id"].ToString().ToInt();

                db.SaveChanges();
            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult saveHotelLocation(HotelLocationViewModel hotelLocationViewModel)
        {
            if (hotelLocationViewModel.id == 0)
            {
                HotelLocation hLoc  = AutoMapper.Mapper.Map<HotelLocationViewModel, HotelLocation>(hotelLocationViewModel);

                hLoc.created_at = DateTime.Now;
                hLoc.created_by = Session["id"].ToString().ToInt();
                db.HotelLocations.Add(hLoc);
                db.SaveChanges();
            }
            else
            {
                HotelLocation hloc = db.HotelLocations.Find(hotelLocationViewModel.id);

                hloc.hotel_id = hotelLocationViewModel.hotel_id;
                hloc.map_link = hotelLocationViewModel.map_link;
                hloc.distance = hotelLocationViewModel.distance;
                hloc.active = 1;

                hloc.updated_at = DateTime.Now;
                hloc.updated_by = Session["id"].ToString().ToInt();

                db.SaveChanges();
            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getHotels(int city_id = 0)
        {
            if(city_id > 0)
            {
                List<HotelViewModel> hotelList = db.Hotels.Where(c => c.city_id == city_id).Select( h => new HotelViewModel { id = h.id, name = h.name }).ToList();
                return Json(new { hotels = hotelList }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { hotels = 0 }, JsonRequestBehavior.AllowGet);

        }

    }
}