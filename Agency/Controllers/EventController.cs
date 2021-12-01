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
using Agency.Security;
namespace Agency.Controllers
{
    [CustomAuthenticationFilter]
    public class EventController : Controller
    {
        // GET: Event
        AgencyDbContext db = new AgencyDbContext();
        public ActionResult Show()
        {
            return View();
        }
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
                var userData = (from event_row in db.Events
                                join location_row in db.Locations on event_row.location_id equals location_row.id into lo
                                from loc in lo.DefaultIfEmpty()
                                select new EventViewModel
                                {
                                    id = event_row.id,
                                    title = event_row.title,
                                    secret_key = event_row.secret_key,
                                    description = event_row.description,
                                    event_from = event_row.event_from,
                                    due_date = event_row.due_date,
                                    advance_reservation_percentage = event_row.advance_reservation_percentage,
                                    tax = event_row.tax,
                                    to = event_row.to,
                                    active = event_row.active,
                                    location_id = event_row.location_id,
                                    location_name = loc.name == null ? "No Location" : loc.name,
                                    hotels = (from event_hotel in db.EventHotels
                                              join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                                              join vendor in db.Vendors on event_hotel.vendor_id equals vendor.id into vendor_loc
                                              from fl in vendor_loc.DefaultIfEmpty()
                                              select new EventHotelViewModel
                                              {
                                                  id = event_hotel.id,
                                                  name = hotel.name,
                                                  hotel_id = event_hotel.hotel_id,
                                                  rate = hotel.rate,
                                                  single_price = event_hotel.single_price,
                                                  double_price = event_hotel.double_price,
                                                  triple_price = event_hotel.triple_price,
                                                  quad_price = event_hotel.quad_price,
                                                  vendor_single_price = event_hotel.vendor_single_price,
                                                  vendor_douple_price = event_hotel.vendor_douple_price,
                                                  vendor_triple_price = event_hotel.vendor_triple_price,
                                                  vendor_quad_price = event_hotel.vendor_quad_price,
                                                  currency = event_hotel.currency,
                                                  event_id = event_hotel.event_id,
                                                  vendor_id = fl.id,
                                                  eventHotelBenefit = db.EventHotelBenefits.Where(f => f.event_hotel_id == event_hotel.id)
                                                    .Select(f => new EventHotelBenefitViewModel { benefit_id = f.benefit_id, name = f.name }).ToList(),
                                                  
                                              }).Where(h=>h.event_id == event_row.id).ToList()

                                });

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    userData = userData.Where(m => m.title.ToLower().Contains(searchValue.ToLower()) || m.id.ToString().ToLower().Contains(searchValue.ToLower()) ||
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
            ViewBag.Locations = db.Locations.Select(s => new { s.id, s.name }).ToList();
            ViewBag.Vendors = db.Vendors.Select(s => new VendorViewModel{
                                    id = s.id,
                                    NamePlusCode = s.code + "-" +s.name
            }).ToList();

            ViewBag.currency = from Currency s in Enum.GetValues(typeof(Currency))
                                 select new { id = ((int)s), name = s.ToString() };
            ViewBag.benefits = from HotelBenefits s in Enum.GetValues(typeof(HotelBenefits))
                                 select new EventHotelBenefitViewModel { id = ((int)s), name = s.ToString() };
            return View();
        }
        [HttpPost]
        public JsonResult saveEvent(EventViewModel eventVM)
        {

            if (eventVM.id == 0)
            {
                Event event_row = AutoMapper.Mapper.Map<EventViewModel, Event>(eventVM);
                event_row.secret_key = Hash.CreateMD5(Guid.NewGuid().ToString());
                event_row.created_at = DateTime.Now;
                event_row.updated_at = DateTime.Now;
                event_row.created_by = Session["id"].ToString().ToInt();
                db.Events.Add(event_row);
                db.SaveChanges();

            }
            else
            {

                Event oldEvent = db.Events.Find(eventVM.id);

                oldEvent.title = eventVM.title;
                oldEvent.description = eventVM.description;
                oldEvent.event_from = eventVM.event_from;
                oldEvent.to = eventVM.to;
                oldEvent.due_date = eventVM.due_date;
                oldEvent.advance_reservation_percentage = eventVM.advance_reservation_percentage;
                oldEvent.tax = eventVM.tax;
                oldEvent.active = eventVM.active;
                oldEvent.location_id = eventVM.location_id;
                oldEvent.updated_by = Session["id"].ToString().ToInt();
                oldEvent.updated_at = DateTime.Now;

                db.Entry(oldEvent).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult saveEventHotel(EventHotelViewModel eventVM)
        {

            if (eventVM.id == 0)
            {
                EventHotel event_row = AutoMapper.Mapper.Map<EventHotelViewModel, EventHotel>(eventVM);

                event_row.created_at = DateTime.Now;
                event_row.updated_at = DateTime.Now;
                event_row.created_by = Session["id"].ToString().ToInt();
                db.EventHotels.Add(event_row);
                db.SaveChanges();

                if (eventVM.benefits != null)
                {
                    foreach (var benefit in eventVM.benefits)
                    {
                        EventHotelBenefit eventHotelBenefit  = new EventHotelBenefit();
                        eventHotelBenefit.event_hotel_id = event_row.id;
                        eventHotelBenefit.benefit_id = benefit;
                        eventHotelBenefit.active = 1;
                        eventHotelBenefit.name = ((HotelBenefits)benefit).ToString();
                        eventHotelBenefit.created_at = DateTime.Now;
                        eventHotelBenefit.created_by = Session["id"].ToString().ToInt();
                        db.EventHotelBenefits.Add(eventHotelBenefit);
                        db.SaveChanges();
                    }
                }
            }
            else
            {

                EventHotel oldEvent = db.EventHotels.Find(eventVM.id);

                oldEvent.hotel_id = eventVM.hotel_id;
                oldEvent.single_price = eventVM.single_price;
                oldEvent.double_price = eventVM.double_price;
                oldEvent.triple_price = eventVM.triple_price;
                oldEvent.quad_price = eventVM.quad_price;
                oldEvent.vendor_single_price = eventVM.vendor_single_price;
                oldEvent.vendor_douple_price = eventVM.vendor_douple_price;
                oldEvent.vendor_triple_price = eventVM.vendor_triple_price;
                oldEvent.vendor_quad_price = eventVM.vendor_quad_price;
                oldEvent.vendor_id = eventVM.vendor_id;
                oldEvent.updated_by = Session["id"].ToString().ToInt();
                oldEvent.updated_at = DateTime.Now;

                db.Entry(oldEvent).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                if (eventVM.benefits != null)
                {
                    List<EventHotelBenefit> eventHotelBenefits = db.EventHotelBenefits.Where(f => f.event_hotel_id == oldEvent.id).ToList();
                    foreach (EventHotelBenefit ben in eventHotelBenefits)
                    {
                        db.EventHotelBenefits.Remove(ben);
                        db.SaveChanges();
                    }
                    foreach (var benefit in eventVM.benefits)
                    {
                        EventHotelBenefit eventHotelBenefit = new EventHotelBenefit();
                        eventHotelBenefit.event_hotel_id = oldEvent.id;
                        eventHotelBenefit.benefit_id = benefit;
                        eventHotelBenefit.active = 1;
                        eventHotelBenefit.name = ((HotelBenefits)benefit).ToString();
                        eventHotelBenefit.created_at = DateTime.Now;
                        eventHotelBenefit.created_by = Session["id"].ToString().ToInt();
                        db.EventHotelBenefits.Add(eventHotelBenefit);
                        db.SaveChanges();
                    }
                }

            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult deleteEvent(int id)
        {
            Event deleteEvent = db.Events.Find(id);
            deleteEvent.active = 0;
            deleteEvent.deleted_at = DateTime.Now;
            deleteEvent.deleted_by = Session["id"].ToString().ToInt();
            db.Entry(deleteEvent).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getHotels(int location_id = 0)
        {
            if (location_id > 0)
            {
                var hotelList = (from hLoc in db.HotelLocations
                                 join hotel in db.Hotels on hLoc.hotel_id equals hotel.id
                                 select new HotelViewModel
                                 {
                                     id = hotel.id,
                                     name = hotel.name,
                                     location_id = hLoc.location_id
                                 }).Where(h => h.location_id == location_id).ToList();

                return Json(new { hotels = hotelList }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { hotels = 0 }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult deleteEventHotel(int id)
        {
            EventHotel deletedEventHotel = db.EventHotels.Find(id);
            db.EventHotels.Remove(deletedEventHotel);
            db.SaveChanges();
            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }
    }
}