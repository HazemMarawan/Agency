using Agency.Auth;
using Agency.Helpers;
using Agency.Models;
using Agency.Enums;
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
    public class HotelController : Controller
    {
        // GET: Hotel
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
                var userData = (from hotel in db.Hotels
                                join city in db.Cities on hotel.city_id equals city.id
                                select new HotelViewModel
                                {
                                    id = hotel.id,
                                    name = hotel.name,
                                    description = hotel.description,
                                    address = hotel.address,
                                    city_id  = hotel.city_id,
                                    rating = hotel.rate,
                                    cityName = city.name,
                                    active = hotel.active,
                                    hotelFacilities =  db.HotelFacilities.Where(f => f.hotel_id == hotel.id)
                                    .Select(f => new HotelFacilitieViewModel { facilitie_id = f.facilitie_id , name = f.name }).ToList(),
                                    HotelImagesVM = db.HotelImages.Where(hi => hi.hotel_id == hotel.id).Select(h => new HotelImageViewModel { id= h.id, hotel_id = h.hotel_id, path = h.path }).ToList()                
                                
                                });

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    userData = userData.Where(m => m.name.ToLower().Contains(searchValue.ToLower()) || m.id.ToString().ToLower().Contains(searchValue.ToLower()) ||
                     m.description.ToLower().Contains(searchValue.ToLower()) || m.cityName.ToString().ToLower().Contains(searchValue.ToLower()));
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

            ViewBag.Cities = db.Cities.Select(s => new { s.id, s.name }).ToList();
            ViewBag.facilities = from HotelFacilities s in Enum.GetValues(typeof(HotelFacilities))
                                 select new HotelFacilitieViewModel { id = ((int)s), name = s.ToString() };

            return View();
        }
        [HttpPost]
        public JsonResult saveHotel(HotelViewModel hotelVM)
        {

            if (hotelVM.id == 0)
            {
                Hotel hotel = AutoMapper.Mapper.Map<HotelViewModel, Hotel>(hotelVM);
                hotel.rate = hotelVM.rating;
                hotel.created_at = DateTime.Now;
                hotel.created_by = Session["id"].ToString().ToInt();
                
                db.Hotels.Add(hotel);
                db.SaveChanges();

                if (hotelVM.facilities != null)
                {
                    foreach(var facility in hotelVM.facilities)
                    {
                        HotelFacilitie hotelFacility = new HotelFacilitie();
                        hotelFacility.hotel_id = hotel.id;
                        hotelFacility.facilitie_id =  facility;
                        hotelFacility.active = 1;
                        hotelFacility.name = ((HotelFacilities)facility).ToString();
                        hotelFacility.created_at = DateTime.Now;
                        hotelFacility.created_by = Session["id"].ToString().ToInt();
                        db.HotelFacilities.Add(hotelFacility);
                        db.SaveChanges();
                    }
                }

                if (hotelVM.images[1] != null)
                {
                    foreach (var image in hotelVM.images)
                    {
                        HotelImage hotelImage = new HotelImage();
                        Guid guid = Guid.NewGuid();
                        var InputFileName = Path.GetFileName(image.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/images/hotels/") + hotel.name + "_" + guid.ToString() + Path.GetExtension(image.FileName));
                        image.SaveAs(ServerSavePath);
                        hotelImage.path = "/images/hotels/" + hotel.name + "_" + guid.ToString() + Path.GetExtension(image.FileName); hotelImage.hotel_id = hotel.id;
                        hotelImage.hotel_id = hotel.id;
                        hotelImage.active = 1;
                        hotelImage.created_at = DateTime.Now;
                        hotelImage.created_by = Session["id"].ToString().ToInt();
                        db.HotelImages.Add(hotelImage);
                        db.SaveChanges();
                    }

                }
            }
            else
            {

                Hotel oldHotel = db.Hotels.Find(hotelVM.id);

                oldHotel.name = hotelVM.name;
                oldHotel.description = hotelVM.description;
                oldHotel.rate = hotelVM.rating;
                oldHotel.address = hotelVM.address;
                oldHotel.active = hotelVM.active;
                oldHotel.updated_by = Session["id"].ToString().ToInt();
                oldHotel.updated_at = DateTime.Now;

                db.Entry(oldHotel).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                if (hotelVM.facilities != null)
                {
                    List<HotelFacilitie> hotelFacilities = db.HotelFacilities.Where(f => f.hotel_id == oldHotel.id).ToList();
                    foreach(HotelFacilitie fac in hotelFacilities)
                    {
                        db.HotelFacilities.Remove(fac);
                        db.SaveChanges();
                    }
                    foreach (var facility in hotelVM.facilities)
                    {
                        HotelFacilitie hotelFacility = new HotelFacilitie();
                        hotelFacility.hotel_id = oldHotel.id;
                        hotelFacility.facilitie_id = facility;
                        hotelFacility.active = 1;
                        hotelFacility.name = ((HotelFacilities)facility).ToString();
                        hotelFacility.created_at = DateTime.Now;
                        hotelFacility.created_by = Session["id"].ToString().ToInt();
                        db.HotelFacilities.Add(hotelFacility);
                        db.SaveChanges();
                    }
                }
                if (hotelVM.images[0] != null)
                {
                    List<HotelImage> hotelImages = db.HotelImages.Where(f => f.hotel_id == oldHotel.id).ToList();
                    foreach (HotelImage img in hotelImages)
                    {
                        db.HotelImages.Remove(img);
                        db.SaveChanges();
                    }
                    foreach (var image in hotelVM.images)
                    {
                        HotelImage hotelImage = new HotelImage();
                        Guid guid = Guid.NewGuid();
                        var InputFileName = Path.GetFileName(image.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/images/hotels/") + oldHotel.name + "_" + guid.ToString() + Path.GetExtension(image.FileName));
                        image.SaveAs(ServerSavePath);
                        hotelImage.path = "/images/hotels/" + oldHotel.name + "_" + guid.ToString() + Path.GetExtension(image.FileName);
                        hotelImage.hotel_id = oldHotel.id;
                        hotelImage.active = 1;
                        hotelImage.created_at = DateTime.Now;
                        hotelImage.created_by = Session["id"].ToString().ToInt();
                        db.HotelImages.Add(hotelImage);
                        db.SaveChanges();
                    }

                }

            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult deleteHotel(int id)
        {
            Hotel deleteHotel = db.Hotels.Find(id);
            db.Hotels.Remove(deleteHotel);
            db.SaveChanges();

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }
    }
}