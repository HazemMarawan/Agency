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
using Agency.Services;

namespace Agency.Controllers
{
    public class BookingController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: Booking
        public ActionResult Event(string secret_key)
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
                                                               id = hotel.id,
                                                               event_hotel_id = event_hotel.id,
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

        public ActionResult Hotel(int id)
        {
            HotelViewModel selectedHotel = (from event_hotel in db.EventHotels
                                          join selected_event in db.Events on event_hotel.event_id equals selected_event.id
                                          join location in db.Locations on selected_event.location_id equals location.id
                                          join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                                          join location_hotel in db.HotelLocations on hotel.id equals location_hotel.hotel_id 
                                          join city in db.Cities on hotel.city_id equals city.id
                                          select new HotelViewModel
                                          {
                                              id = hotel.id,
                                              event_hotel_id = event_hotel.id,
                                              name = hotel.name,
                                              description = hotel.description,
                                              address = hotel.address,
                                              city_id = hotel.city_id,
                                              rating = hotel.rate,
                                              map_link = location_hotel.map_link,
                                              cityName = city.name,
                                              active = hotel.active,
                                              hotelFacilities = db.HotelFacilities.Where(f => f.hotel_id == hotel.id).Select(f => new HotelFacilitieViewModel { facilitie_id = f.facilitie_id, name = f.name }).ToList(),
                                              HotelImagesVM = db.HotelImages.Where(hi => hi.hotel_id == hotel.id).Select(h => new HotelImageViewModel { id = h.id, hotel_id = h.hotel_id, path = h.path }).ToList(),
                                              title =selected_event.title,
                                              event_description = selected_event.description,
                                              event_from = selected_event.event_from,
                                              event_to = selected_event.to,
                                              event_location = location.name,
                                              hotel_location_distance = location_hotel.distance,
                                              location_id = location_hotel.location_id,
                                              event_location_id = location.id,
                                              single_price = event_hotel.single_price,
                                              double_price = event_hotel.double_price,
                                              currency = event_hotel.currency
                                          }).Where(s => s.event_hotel_id == id && s.location_id == s.event_location_id).FirstOrDefault();
            
            return View(selectedHotel);
        }

        public ActionResult makeReservation(int event_hotel_id,int number_of_rooms)
        {
            HotelViewModel selectedHotel = (from event_hotel in db.EventHotels
                                            join selected_event in db.Events on event_hotel.event_id equals selected_event.id
                                            join location in db.Locations on selected_event.location_id equals location.id
                                            join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                                            join location_hotel in db.HotelLocations on hotel.id equals location_hotel.hotel_id
                                            join city in db.Cities on hotel.city_id equals city.id
                                            select new HotelViewModel
                                            {
                                                id = hotel.id,
                                                event_id = event_hotel.event_id,
                                                event_hotel_id = event_hotel.id,
                                                name = hotel.name,
                                                description = hotel.description,
                                                address = hotel.address,
                                                city_id = hotel.city_id,
                                                rating = hotel.rate,
                                                map_link = location_hotel.map_link,
                                                cityName = city.name,
                                                tax =selected_event.tax,
                                                single_price = event_hotel.single_price,
                                                double_price = event_hotel.double_price,
                                                triple_price = event_hotel.triple_price,
                                                quad_price = event_hotel.quad_price,
                                                advance_reservation_percentage = selected_event.advance_reservation_percentage,
                                                active = hotel.active,
                                                hotelFacilities = db.HotelFacilities.Where(f => f.hotel_id == hotel.id).Select(f => new HotelFacilitieViewModel { facilitie_id = f.facilitie_id, name = f.name }).ToList(),
                                                HotelImagesVM = db.HotelImages.Where(hi => hi.hotel_id == hotel.id).Select(h => new HotelImageViewModel { id = h.id, hotel_id = h.hotel_id, path = h.path }).ToList(),
                                                title = selected_event.title,
                                                event_description = selected_event.description,
                                                event_from = selected_event.event_from,
                                                event_to = selected_event.to,
                                                event_location = location.name,
                                                hotel_location_distance = location_hotel.distance,
                                                location_id = location_hotel.location_id,
                                                event_location_id = location.id
                                            }).Where(s => s.event_hotel_id == event_hotel_id && s.location_id == s.event_location_id).FirstOrDefault();
            ViewBag.NumberOfRooms = number_of_rooms;
            return View(selectedHotel);
        }
        [HttpPost]
        public ActionResult makeReservation(ReservationViewModel reservationViewModel)
        {
            
            Reservation reservation = AutoMapper.Mapper.Map<ReservationViewModel, Reservation>(reservationViewModel);
            EventHotel eventHotel = db.EventHotels.Find(reservation.event_hotel_id);
            Event c_event = db.Events.Find(eventHotel.event_id);

            reservation.single_price = eventHotel.single_price;
            reservation.vendor_single_price = eventHotel.vendor_single_price;
            reservation.double_price = eventHotel.double_price;
            reservation.vendor_douple_price = eventHotel.vendor_douple_price;
            reservation.triple_price = eventHotel.triple_price;
            reservation.vendor_triple_price = eventHotel.vendor_triple_price;
            reservation.quad_price = eventHotel.quad_price;
            reservation.vendor_quad_price = eventHotel.vendor_quad_price;
            reservation.tax = db.Events.Find(eventHotel.event_id).tax;
            reservation.currency = eventHotel.currency;
            reservation.tax_amount = 0;
            reservation.total_amount_after_tax = 0;
            reservation.total_amount_from_vendor = 0;
            reservation.total_amount = 0;
            reservation.paid_amount = 0;
            reservation.financial_advance = 0;
            reservation.financial_due = 0;
            reservation.advance_reservation_percentage = c_event.advance_reservation_percentage;       
            reservation.vendor_id = eventHotel.vendor_id;
            reservation.created_at = DateTime.Now;
            reservation.updated_at = DateTime.Now;
            reservation.balance_due_date = null;
            reservation.active = 1;
            db.Reservations.Add(reservation);
            db.SaveChanges();


            ReservationCreditCard reservationCreditCard = new ReservationCreditCard();
            reservationCreditCard.reservation_id = reservation.id;
            reservationCreditCard.credit_card_number = reservationViewModel.credit_card_number;
            reservationCreditCard.security_code = reservationViewModel.security_code;
            reservationCreditCard.card_expiration_date = reservationViewModel.security_code;
            reservationCreditCard.created_at = DateTime.Now;
            db.ReservationCreditCards.Add(reservationCreditCard);
            db.SaveChanges();



            Company company = new Company();
            company.name = reservationViewModel.company_name;
            company.created_at = DateTime.Now;
            company.updated_at = DateTime.Now;
            //company.created_by = Session["id"].ToString().ToInt();
            db.Companies.Add(company);
            db.SaveChanges();

            reservation.company_id = company.id;
            db.SaveChanges();

            DateTime minCheckin = reservationViewModel.reservation_from.Min();
            DateTime maxCheckout = reservationViewModel.reservation_to.Min();
            double? total_amount_from_vendor = 0;
            for (int i=0;i< reservationViewModel.first_name.Count;i++)
            { 
                double? room_price = 0;
                double? vendor_room_price = 0;
                ReservationDetail detail = new ReservationDetail();

                if (reservationViewModel.room_type[i] == 1)
                {
                    room_price = reservation.single_price;
                    vendor_room_price = reservation.vendor_single_price;
                  
                }
                else if (reservationViewModel.room_type[i] == 2)
                {
                    room_price = reservation.double_price;
                    vendor_room_price = reservation.vendor_douple_price;
                }
                else if (reservationViewModel.room_type[i] == 3)
                {
                    room_price = reservation.triple_price;
                    vendor_room_price = reservation.vendor_triple_price;
                }
                else if (reservationViewModel.room_type[i] == 4)
                {
                    room_price = reservation.quad_price;
                    vendor_room_price = reservation.vendor_quad_price;
                }
                else
                {
                    vendor_room_price = reservation.vendor_single_price;
                }
                int Days = (reservationViewModel.reservation_to[i] - reservationViewModel.reservation_from[i]).Days;
                detail.no_of_days = Days+1;

                var amount = (room_price * Days);
                var vendor_amount = (vendor_room_price * Days);
                detail.reservation_id = reservation.id;
                detail.tax = amount * (reservation.tax / 100);
                detail.amount = amount;
                detail.amount_after_tax = amount + detail.tax;
                detail.created_at = DateTime.Now;
                detail.reservation_from = reservationViewModel.reservation_from[i];
                detail.reservation_to = reservationViewModel.reservation_to[i];
                detail.room_type = reservationViewModel.room_type[i];
                detail.active = 1;
                detail.payment_to_vendor_deadline = detail.paid_to_vendor_date = detail.payment_to_vendor_notification_date = detail.deleted_at = detail.updated_at = null;
                detail.vendor_cost = vendor_room_price;
                db.ReservationDetails.Add(detail);
                db.SaveChanges();

                detail.parent_id = detail.id;
                db.SaveChanges();

                Client client = new Client();
                client.first_name = reservationViewModel.first_name[i];
                client.last_name = reservationViewModel.last_name[i];
                client.company_id = reservation.company_id;
                client.active = 1;
                client.created_at = DateTime.Now;
                db.Clients.Add(client);
                db.SaveChanges();

                detail.client_id = client.id;
                db.SaveChanges();

                total_amount_from_vendor += vendor_room_price;
            }

            List<ReservationDetail> reservationDetails = db.ReservationDetails.Where(r => r.reservation_id == reservation.id).ToList();
            if (reservationDetails.Count() != 0)
            {
                DateTime minDate = reservationDetails.Select(s => s.reservation_from).Min();
                DateTime maxDate = reservationDetails.Select(s => s.reservation_to).Max();

                reservation.check_in = minDate;
                reservation.check_out = maxDate;
                reservation.balance_due_date = Convert.ToDateTime(reservation.check_in).AddDays(-30);
                reservation.total_rooms = reservationViewModel.first_name.Count;

                ReservationService.UpdateTotals(reservation.id,true);
            }

            InitialReservation initialReservation = AutoMapper.Mapper.Map<Reservation, InitialReservation>(reservation);
            initialReservation.id = 0;
            initialReservation.reservation_id = reservation.id;
            db.InitialReservations.Add(initialReservation);
            db.SaveChanges();

            EventHotel currentEventHotel = db.EventHotels.Find(reservation.event_hotel_id);
            Event currentEvent = db.Events.Find(currentEventHotel.event_id);
            return RedirectToAction("Event", new { secret_key = currentEvent.secret_key });
        }
    }
}