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
    public class BookingController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: Booking
        public ActionResult Show(string secret_key)
        {
            EventViewModel eventHotels = (from event_row in db.Events
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
                                                 hotels = (from hotel in db.Hotels
                                                           join event_hotel in db.EventHotels on hotel.id equals event_hotel.hotel_id
                                                           join vendor in db.Vendors on event_hotel.vendor_id equals vendor.id into vendor_loc
                                                           from fl in vendor_loc.DefaultIfEmpty()
                                                           join hotel_location in db.HotelLocations on hotel.id equals hotel_location.hotel_id
                                                           join location in db.Locations on hotel_location.location_id equals location.id
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
                                                               fixed_rate = (int)hotel.rate,
                                                               location_id = location.id,
                                                               distance = hotel_location.distance,
                                                               eventHotelBenefit = db.EventHotelBenefits.Where(f => f.event_hotel_id == event_hotel.id)
                                                                 .Select(f => new EventHotelBenefitViewModel { benefit_id = f.benefit_id, name = f.name }).ToList(),
                                                               hotelImages = db.HotelImages.Where(s => s.hotel_id == event_hotel.hotel_id).Select(s => new HotelImageViewModel
                                                               {
                                                                   id = s.id,
                                                                   path = s.path
                                                               }).ToList()

                                                           }).Where(h => h.event_id == event_row.id && h.location_id == event_row.location_id).ToList()

                                             }).Where(s=>s.secret_key == secret_key).FirstOrDefault();
            return View(eventHotels);
        }
    }
}