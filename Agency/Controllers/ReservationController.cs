using Agency.Auth;
using Agency.Enums;
using Agency.Helpers;
using Agency.Models;
using Agency.ViewModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Agency.Services;
namespace Agency.Controllers
{
    [CustomAuthenticationFilter]
    public class ReservationController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();

        // GET: Reservation
        public ActionResult Index()
        {
            ViewBag.Events = db.Events.Select(s => new { s.id, s.title, s.updated_at }).OrderByDescending(e => e.updated_at).ToList();

            ViewBag.Opener = db.Users.Select(s => new { s.id, s.full_name }).ToList();
            ViewBag.Closer = db.Users.Select(s => new { s.id, s.full_name }).ToList();
            ViewBag.Vendors = db.Vendors.Select(s => new VendorViewModel
            {
                id = s.id,
                NamePlusCode = s.code + "-" + s.name
            }).ToList();

            ViewBag.currency = from Currency s in Enum.GetValues(typeof(Currency))
                               select new { id = ((int)s), name = s.ToString() };

            ViewBag.Shift = from Shift s in Enum.GetValues(typeof(Shift))
                                 select new ReservationViewModel { id = ((int)s), shift_name = s.ToString() };
            return View();
        }
        public ActionResult View(int id)
        {
            var resData = (from res in db.Reservations
                           join com in db.Companies on res.company_id equals com.id into cm
                           from company in cm.DefaultIfEmpty()
                           join even_hot in db.EventHotels on res.event_hotel_id equals even_hot.id into eh
                           from event_hotel in eh.DefaultIfEmpty()
                           join eve in db.Events on event_hotel.event_id equals eve.id into ev
                           from even in ev.DefaultIfEmpty()
                           join location in db.Locations on even.location_id equals location.id into lc
                           from loc in lc.DefaultIfEmpty()
                           join cit in db.Cities on loc.city_id equals cit.id into ci
                           from city in ci.DefaultIfEmpty()
                           join hot in db.Hotels on event_hotel.hotel_id equals hot.id into ho
                           from hotel in ho.DefaultIfEmpty()
                           join vend in db.Vendors on event_hotel.vendor_id equals vend.id into  ven
                           from vendor in ven.DefaultIfEmpty()
                           join oU in db.Users on res.opener equals oU.id into us
                           from opener in us.DefaultIfEmpty()
                           join cU in db.Users on res.closer equals cU.id into use
                           from closer in use.DefaultIfEmpty()
                           join usr in db.Users on res.created_by equals usr.id into ucr
                           from createdBy in ucr.DefaultIfEmpty()
                           join uBy in db.Users on res.updated_by equals uBy.id into byU
                           from updatedBy in byU.DefaultIfEmpty()
                           select new ReservationViewModel
                           {
                               id = res.id,
                               total_amount = res.total_amount,
                               currency = res.currency,
                               string_currency = ((Currency)res.currency).ToString(),
                               tax = res.tax,
                               financial_advance = res.financial_advance,
                               financial_advance_date = res.financial_advance_date,
                               financial_due = res.financial_due,
                               financial_due_date = res.financial_due_date,
                               status = res.status,
                               single_price = res.single_price,
                               double_price = res.double_price,
                               triple_price = res.triple_price,
                               quad_price = res.quad_price,
                               vendor_single_price = res.vendor_single_price,
                               vendor_douple_price = res.vendor_douple_price,
                               vendor_triple_price = res.vendor_triple_price,
                               vendor_quad_price = res.vendor_quad_price,
                               vendor_id = res.vendor_id,
                               active = res.active,
                               company_name = company.name,
                               phone = company.phone,
                               email = company.email,
                               company_id = res.company_id,
                               event_hotel_id = event_hotel.id,
                               hotel_name = hotel.name,
                               hotel_rate = hotel.rate,
                               reservations_officer_name = res.reservations_officer_name,
                               reservations_officer_email = res.reservations_officer_email,
                               reservations_officer_phone = res.reservations_officer_phone,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "-" : opener.full_name,
                               closer_name = opener.full_name == null ? "-" : closer.full_name,
                               check_in = res.check_in,
                               check_out = res.check_out,
                               string_check_in = res.check_in.ToString(),
                               string_check_out = res.check_out.ToString(),
                               total_nights = res.total_nights,
                               total_rooms = db.ReservationDetails.Where(rd=>rd.reservation_id == res.id).Count(),
                               profit = res.profit,
                               shift = res.shift,
                               created_at_string = res.created_at.ToString(),
                               updated_at_string = res.updated_at.ToString(),
                               updated_by = res.updated_by,
                               event_name = even.title,
                               event_tax = even.tax,
                               reservation_avg_price = db.ReservationDetails.Where(rsd=>rsd.reservation_id == res.id).Select(s=>s.amount).Sum()/res.total_nights,
                               vendor_avg_price = db.ReservationDetails.Where(rsd=>rsd.reservation_id == res.id).Select(s=>s.vendor_cost).Sum()/res.total_nights,
                               event_single_price = event_hotel.single_price,
                               event_double_price = event_hotel.double_price,
                               event_triple_price = event_hotel.triple_price,
                               event_quad_price = event_hotel.quad_price,
                               event_vendor_single_price = event_hotel.vendor_single_price,
                               event_vendor_double_price = event_hotel.vendor_douple_price,
                               event_vendor_triple_price = event_hotel.vendor_triple_price,
                               event_vendor_quad_price = event_hotel.vendor_quad_price,
                               total_price = db.ReservationDetails.Where(r => r.reservation_id == res.id).Select(p => p.amount).Sum(),
                               paid_amount = res.paid_amount,
                               total_amount_after_tax = res.total_amount_after_tax,
                               total_amount_from_vendor = res.total_amount_from_vendor,
                               advance_reservation_percentage = res.advance_reservation_percentage,
                               tax_amount = res.tax_amount,
                               created_by_name = createdBy.full_name,
                               updated_by_name = updatedBy.full_name == null ? "Not Updated":updatedBy.full_name,
                               shift_name = ((Shift)res.shift).ToString(),
                               location_name = loc.name,
                               city_name = city.name,
                               vendor_code = vendor.code,
                               card_expiration_date = res.card_expiration_date,
                               credit_card_number = res.credit_card_number,
                               security_code = res.security_code
                               //profit = calculateProfit(res.id).profit
                           }).Where(r => r.id == id).FirstOrDefault();

            var initialResData = (from reinitialRes in db.InitialReservations
                           join com in db.Companies on reinitialRes.company_id equals com.id into cm
                           from company in cm.DefaultIfEmpty()
                           join even_hot in db.EventHotels on reinitialRes.event_hotel_id equals even_hot.id into eh
                           from event_hotel in eh.DefaultIfEmpty()
                           join eve in db.Events on event_hotel.event_id equals eve.id into ev
                           from even in ev.DefaultIfEmpty()
                           join location in db.Locations on even.location_id equals location.id into lc
                           from loc in lc.DefaultIfEmpty()
                           join cit in db.Cities on loc.city_id equals cit.id into ci
                           from city in ci.DefaultIfEmpty()
                           join hot in db.Hotels on event_hotel.hotel_id equals hot.id into ho
                           from hotel in ho.DefaultIfEmpty()
                           join vend in db.Vendors on event_hotel.vendor_id equals vend.id into ven
                           from vendor in ven.DefaultIfEmpty()
                           join oU in db.Users on reinitialRes.opener equals oU.id into us
                           from opener in us.DefaultIfEmpty()
                           join cU in db.Users on reinitialRes.closer equals cU.id into use
                           from closer in use.DefaultIfEmpty()
                           join usr in db.Users on reinitialRes.created_by equals usr.id into ucr
                           from createdBy in ucr.DefaultIfEmpty()
                           join uBy in db.Users on reinitialRes.updated_by equals uBy.id into byU
                           from updatedBy in byU.DefaultIfEmpty()
                           select new ReservationViewModel
                           {
                               reservation_id = reinitialRes.reservation_id,
                               total_amount = reinitialRes.total_amount,
                               currency = reinitialRes.currency,
                               string_currency = ((Currency)reinitialRes.currency).ToString(),
                               tax = reinitialRes.tax,
                               financial_advance = reinitialRes.financial_advance,
                               financial_advance_date = reinitialRes.financial_advance_date,
                               financial_due = reinitialRes.financial_due,
                               financial_due_date = reinitialRes.financial_due_date,
                               status = reinitialRes.status,
                               single_price = reinitialRes.single_price,
                               double_price = reinitialRes.double_price,
                               triple_price = reinitialRes.triple_price,
                               quad_price = reinitialRes.quad_price,
                               vendor_single_price = reinitialRes.vendor_single_price,
                               vendor_douple_price = reinitialRes.vendor_douple_price,
                               vendor_triple_price = reinitialRes.vendor_triple_price,
                               vendor_quad_price = reinitialRes.vendor_quad_price,
                               vendor_id = reinitialRes.vendor_id,
                               active = reinitialRes.active,
                               company_name = company.name,
                               phone = company.phone,
                               email = company.email,
                               company_id = reinitialRes.company_id,
                               event_hotel_id = event_hotel.id,
                               hotel_name = hotel.name,
                               hotel_rate = hotel.rate,
                               reservations_officer_name = reinitialRes.reservations_officer_name,
                               reservations_officer_email = reinitialRes.reservations_officer_email,
                               reservations_officer_phone = reinitialRes.reservations_officer_phone,
                               opener = reinitialRes.opener,
                               closer = reinitialRes.closer,
                               opener_name = opener.full_name == null ? "-" : opener.full_name,
                               closer_name = opener.full_name == null ? "-" : closer.full_name,
                               check_in = reinitialRes.check_in,
                               check_out = reinitialRes.check_out,
                               string_check_in = reinitialRes.check_in.ToString(),
                               string_check_out = reinitialRes.check_out.ToString(),
                               total_nights = reinitialRes.total_nights,
                               total_rooms = db.ReservationDetails.Where(rd => rd.reservation_id == reinitialRes.reservation_id).Count(),
                               profit = reinitialRes.profit,
                               shift = reinitialRes.shift,
                               created_at_string = reinitialRes.created_at.ToString(),
                               updated_at_string = reinitialRes.updated_at.ToString(),
                               updated_by = reinitialRes.updated_by,
                               event_name = even.title,
                               event_tax = even.tax,
                               reservation_avg_price = db.ReservationDetails.Where(rsd => rsd.reservation_id == reinitialRes.reservation_id).Select(s => s.amount).Sum() / reinitialRes.total_nights,
                               vendor_avg_price = db.ReservationDetails.Where(rsd => rsd.reservation_id == reinitialRes.reservation_id).Select(s => s.vendor_cost).Sum() / reinitialRes.total_nights,
                               event_single_price = event_hotel.single_price,
                               event_double_price = event_hotel.double_price,
                               event_triple_price = event_hotel.triple_price,
                               event_quad_price = event_hotel.quad_price,
                               event_vendor_single_price = event_hotel.vendor_single_price,
                               event_vendor_double_price = event_hotel.vendor_douple_price,
                               event_vendor_triple_price = event_hotel.vendor_triple_price,
                               event_vendor_quad_price = event_hotel.vendor_quad_price,
                               total_price = db.ReservationDetails.Where(r => r.reservation_id == reinitialRes.reservation_id).Select(p => p.amount).Sum(),
                               paid_amount = reinitialRes.paid_amount,
                               total_amount_after_tax = reinitialRes.total_amount_after_tax,
                               total_amount_from_vendor = reinitialRes.total_amount_from_vendor,
                               advance_reservation_percentage = reinitialRes.advance_reservation_percentage,
                               tax_amount = reinitialRes.tax_amount,
                               created_by_name = createdBy.full_name,
                               updated_by_name = updatedBy.full_name == null ? "Not Updated" : updatedBy.full_name,
                               shift_name = ((Shift)reinitialRes.shift).ToString(),
                               location_name = loc.name,
                               city_name = city.name,
                               vendor_code = vendor.code,
                               card_expiration_date = reinitialRes.card_expiration_date,
                               credit_card_number = reinitialRes.credit_card_number,
                               security_code = reinitialRes.security_code
                               //profit = calculateProfit(res.id).profit
                           }).Where(r => r.reservation_id == id).FirstOrDefault();

            ReservationViewScreenViewModel reservationViewScreenViewModel = new ReservationViewScreenViewModel();
            reservationViewScreenViewModel.CurrentReservation = resData;
            reservationViewScreenViewModel.InitialReservation = initialResData;


            ViewBag.id = id;
            ViewBag.Users = db.Users.Select(s => new { s.id, s.full_name }).ToList();
            ViewBag.Events = db.Events.Select(s => new { s.id, s.title, s.updated_at }).OrderByDescending(e => e.updated_at).ToList();

            //ViewBag.Opener = db.Users.Select(s => new { s.id, s.full_name }).ToList();
            ViewBag.Opener = db.Users.Select(s => new UserViewModel { id = s.id, full_name = s.full_name }).ToList();
            ViewBag.Closer = db.Users.Select(s => new UserViewModel { id = s.id, full_name = s.full_name }).ToList();
            ViewBag.Vendors = db.Vendors.Select(s => new VendorViewModel
            {
                id = s.id,
                NamePlusCode = s.code + "-" + s.name
            }).ToList();

            ViewBag.currency = from Currency s in Enum.GetValues(typeof(Currency))
                               select new ReservationViewModel { id = ((int)s), string_currency = s.ToString() };

            ViewBag.Shift = from Shift s in Enum.GetValues(typeof(Shift))
                            select new ReservationViewModel { id = ((int)s), shift_name = s.ToString() };
            return View(reservationViewScreenViewModel);
        }
        public ReservationViewModel calculateTotalandVendor(int res_id)
        {
            double? total_amount = 0;
            double? total_amount_from_vendor = 0;
            Reservation reservation = db.Reservations.Find(res_id);
            List<ReservationDetail> reservationDetails = db.ReservationDetails.Where(rd => rd.reservation_id == res_id).ToList();
            //Event event_obj = db.Events.Find(even_id);
            EventHotel eventHotel = db.EventHotels.Find(reservation.event_hotel_id);
            foreach(var resDetail in reservationDetails)
            {
                
                double? room_price = 0;
                double? vendor_room_price = 0;
                if (resDetail.room_type == 1)
                {
                    room_price = reservation.single_price > 0 ? reservation.single_price : eventHotel.single_price;
                    vendor_room_price = reservation.vendor_single_price > 0 ? reservation.vendor_single_price : eventHotel.vendor_single_price;
                }
                else if(resDetail.room_type == 2)
                {
                    room_price = reservation.double_price;
                    vendor_room_price = reservation.vendor_douple_price > 0 ? reservation.vendor_douple_price : eventHotel.vendor_douple_price;

                }
                else if (resDetail.room_type == 3)
                {
                    room_price = reservation.triple_price;
                    vendor_room_price = reservation.vendor_triple_price > 0 ? reservation.vendor_triple_price : eventHotel.vendor_triple_price;

                }
                else if (resDetail.room_type == 4)
                {
                    room_price = reservation.quad_price;
                    vendor_room_price = reservation.vendor_quad_price > 0 ? reservation.vendor_quad_price : eventHotel.vendor_quad_price;

                }

                total_amount += ((room_price * resDetail.no_of_days) + ((room_price * resDetail.no_of_days)*(reservation.tax/100)));
                total_amount_from_vendor += (vendor_room_price * resDetail.no_of_days);

            }
            ReservationViewModel reservationViewModel = new ReservationViewModel();
            reservationViewModel.total_amount = total_amount;
            reservationViewModel.total_amount_from_vendor = total_amount_from_vendor;

            return reservationViewModel;
        }
        public JsonResult unPaidReservations()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var from_date = Request.Form.GetValues("columns[0][search][value]")[0];
            var to_date = Request.Form.GetValues("columns[1][search][value]")[0];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var resData = (from res in db.Reservations
                           join company in db.Companies on res.company_id equals company.id
                           join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                           join even in db.Events on event_hotel.event_id equals even.id
                           join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                           join oU in db.Users on res.opener equals oU.id into us
                           from opener in us.DefaultIfEmpty()
                           join cU in db.Users on res.closer equals cU.id into use
                           from closer in use.DefaultIfEmpty()
                           select new ReservationViewModel
                           {
                               id = res.id,
                               total_amount = res.total_amount,
                               currency = res.currency,
                               tax = res.tax,
                               financial_advance = res.financial_advance,
                               financial_advance_date = res.financial_advance_date,
                               financial_due = res.financial_due,
                               financial_due_date = res.financial_due_date,
                               status = res.status,
                               single_price = res.single_price,
                               double_price = res.double_price,
                               triple_price = res.triple_price,
                               quad_price = res.quad_price,
                               vendor_single_price = res.vendor_single_price,
                               vendor_douple_price = res.vendor_douple_price,
                               vendor_triple_price = res.vendor_triple_price,
                               vendor_quad_price = res.vendor_quad_price,
                               vendor_id = res.vendor_id,
                               active = res.active,
                               company_name = company.name,
                               phone = company.phone,
                               email = company.email,
                               company_id = res.company_id,
                               event_hotel_id = event_hotel.id,
                               hotel_name = hotel.name,
                               hotel_rate = hotel.rate,
                               reservations_officer_name = res.reservations_officer_name,
                               reservations_officer_email = res.reservations_officer_email,
                               reservations_officer_phone = res.reservations_officer_phone,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = opener.full_name == null ? "No Closer Assigned" : closer.full_name,
                               check_in = res.check_in,
                               check_out = res.check_out,
                               string_check_in = res.check_in.ToString(),
                               string_check_out = res.check_out.ToString(),
                               total_nights = res.total_nights,
                               total_rooms = res.total_rooms,
                               profit = res.profit,
                               shift = res.shift,
                               created_at_string = res.created_at.ToString(),
                               event_name = even.title,
                               event_tax = even.tax,
                               event_single_price = event_hotel.single_price,
                               event_double_price = event_hotel.double_price,
                               event_triple_price = event_hotel.triple_price,
                               event_quad_price = event_hotel.quad_price,
                               event_vendor_single_price = event_hotel.vendor_single_price,
                               event_vendor_double_price = event_hotel.vendor_douple_price,
                               event_vendor_triple_price = event_hotel.vendor_triple_price,
                               event_vendor_quad_price = event_hotel.vendor_quad_price,
                               total_price = db.ReservationDetails.Where(r => r.reservation_id == res.id).Select(p => p.amount).Sum(),
                               paid_amount = res.paid_amount,
                               total_amount_after_tax = res.total_amount_after_tax,
                               total_amount_from_vendor = res.total_amount_from_vendor,
                               advance_reservation_percentage = res.advance_reservation_percentage,
                               tax_amount = res.tax_amount,
                               is_canceled = res.is_canceled
                               //profit = calculateProfit(res.id).profit
                           }).Where(r => r.paid_amount == 0 && r.is_canceled == null);
            //var reservation = resData.ToList();
            //double? collected = 0.0;
            //double? balance = 0.0;
            //double? profit = 0.0;
            //if (reservation != null)
            //{
            //    collected = resData.Select(t => t.paid_amount).ToList().Sum();
            //    balance = resData.Select(t => t.total_amount_after_tax).ToList().Sum() - collected;
            //    profit = resData.Select(t => t.total_amount_after_tax).ToList().Sum() - resData.Select(t => t.total_amount_from_vendor).ToList().Sum();

            //}

            var displayResult = resData.OrderByDescending(u => u.id).Skip(skip)
                 .Take(pageSize).ToList();
            var totalRecords = resData.Count();

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = displayResult,
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult partiallyPaidReservations()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var from_date = Request.Form.GetValues("columns[0][search][value]")[0];
            var to_date = Request.Form.GetValues("columns[1][search][value]")[0];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var resData = (from res in db.Reservations
                           join company in db.Companies on res.company_id equals company.id
                           join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                           join even in db.Events on event_hotel.event_id equals even.id
                           join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                           join oU in db.Users on res.opener equals oU.id into us
                           from opener in us.DefaultIfEmpty()
                           join cU in db.Users on res.closer equals cU.id into use
                           from closer in use.DefaultIfEmpty()
                           select new ReservationViewModel
                           {
                               id = res.id,
                               total_amount = res.total_amount,
                               currency = res.currency,
                               tax = res.tax,
                               financial_advance = res.financial_advance,
                               financial_advance_date = res.financial_advance_date,
                               financial_due = res.financial_due,
                               financial_due_date = res.financial_due_date,
                               status = res.status,
                               single_price = res.single_price,
                               double_price = res.double_price,
                               triple_price = res.triple_price,
                               quad_price = res.quad_price,
                               vendor_single_price = res.vendor_single_price,
                               vendor_douple_price = res.vendor_douple_price,
                               vendor_triple_price = res.vendor_triple_price,
                               vendor_quad_price = res.vendor_quad_price,
                               vendor_id = res.vendor_id,
                               active = res.active,
                               company_name = company.name,
                               phone = company.phone,
                               email = company.email,
                               company_id = res.company_id,
                               event_hotel_id = event_hotel.id,
                               hotel_name = hotel.name,
                               hotel_rate = hotel.rate,
                               reservations_officer_name = res.reservations_officer_name,
                               reservations_officer_email = res.reservations_officer_email,
                               reservations_officer_phone = res.reservations_officer_phone,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = opener.full_name == null ? "No Closer Assigned" : closer.full_name,
                               check_in = res.check_in,
                               check_out = res.check_out,
                               string_check_in = res.check_in.ToString(),
                               string_check_out = res.check_out.ToString(),
                               total_nights = res.total_nights,
                               total_rooms = res.total_rooms,
                               profit = res.profit,
                               shift = res.shift,
                               event_name = even.title,
                               event_tax = even.tax,
                               event_single_price = event_hotel.single_price,
                               event_double_price = event_hotel.double_price,
                               event_triple_price = event_hotel.triple_price,
                               event_quad_price = event_hotel.quad_price,
                               event_vendor_single_price = event_hotel.vendor_single_price,
                               event_vendor_double_price = event_hotel.vendor_douple_price,
                               event_vendor_triple_price = event_hotel.vendor_triple_price,
                               event_vendor_quad_price = event_hotel.vendor_quad_price,
                               total_price = db.ReservationDetails.Where(r => r.reservation_id == res.id).Select(p => p.amount).Sum(),
                               paid_amount = res.paid_amount,
                               total_amount_after_tax = res.total_amount_after_tax,
                               total_amount_from_vendor = res.total_amount_from_vendor,
                               advance_reservation_percentage = res.advance_reservation_percentage,
                               tax_amount = res.tax_amount,
                               is_canceled = res.is_canceled
                               //profit = calculateProfit(res.id).profit

                           }).Where(r=>r.paid_amount < r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled == null);
            //var reservation = resData.ToList();
            //double? collected = 0.0;
            //double? balance = 0.0;
            //double? profit = 0.0;
            //if (reservation != null)
            //{
            //    collected = resData.Select(t => t.paid_amount).ToList().Sum();
            //    balance = resData.Select(t => t.total_amount_after_tax).ToList().Sum() - collected;
            //    profit = resData.Select(t => t.total_amount_after_tax).ToList().Sum() - resData.Select(t => t.total_amount_from_vendor).ToList().Sum();

            //}
            var displayResult = resData.OrderByDescending(u => u.id).Skip(skip)
                 .Take(pageSize).ToList();
            var totalRecords = resData.Count();

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = displayResult,
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PaidReservations()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var from_date = Request.Form.GetValues("columns[0][search][value]")[0];
            var to_date = Request.Form.GetValues("columns[1][search][value]")[0];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var resData = (from res in db.Reservations
                           join company in db.Companies on res.company_id equals company.id
                           join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                           join even in db.Events on event_hotel.event_id equals even.id
                           join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                           join oU in db.Users on res.opener equals oU.id into us
                           from opener in us.DefaultIfEmpty()
                           join cU in db.Users on res.closer equals cU.id into use
                           from closer in use.DefaultIfEmpty()
                           select new ReservationViewModel
                           {
                               id = res.id,
                               total_amount = res.total_amount,
                               currency = res.currency,
                               tax = res.tax,
                               financial_advance = res.financial_advance,
                               financial_advance_date = res.financial_advance_date,
                               financial_due = res.financial_due,
                               financial_due_date = res.financial_due_date,
                               financial_due_date_string = res.financial_due_date.ToString(),
                               status = res.status,
                               single_price = res.single_price,
                               double_price = res.double_price,
                               triple_price = res.triple_price,
                               quad_price = res.quad_price,
                               vendor_single_price = res.vendor_single_price,
                               vendor_douple_price = res.vendor_douple_price,
                               vendor_triple_price = res.vendor_triple_price,
                               vendor_quad_price = res.vendor_quad_price,
                               vendor_id = res.vendor_id,
                               active = res.active,
                               company_name = company.name,
                               phone = company.phone,
                               email = company.email,
                               company_id = res.company_id,
                               event_hotel_id = event_hotel.id,
                               hotel_name = hotel.name,
                               hotel_rate = hotel.rate,
                               reservations_officer_name = res.reservations_officer_name,
                               reservations_officer_email = res.reservations_officer_email,
                               reservations_officer_phone = res.reservations_officer_phone,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = opener.full_name == null ? "No Closer Assigned" : closer.full_name,
                               check_in = res.check_in,
                               check_out = res.check_out,
                               string_check_in = res.check_in.ToString(),
                               string_check_out = res.check_out.ToString(),
                               total_nights = res.total_nights,
                               total_rooms = res.total_rooms,
                               profit = res.profit,
                               shift = res.shift,
                               event_name = even.title,
                               event_tax = even.tax,
                               event_single_price = event_hotel.single_price,
                               event_double_price = event_hotel.double_price,
                               event_triple_price = event_hotel.triple_price,
                               event_quad_price = event_hotel.quad_price,
                               event_vendor_single_price = event_hotel.vendor_single_price,
                               event_vendor_double_price = event_hotel.vendor_douple_price,
                               event_vendor_triple_price = event_hotel.vendor_triple_price,
                               event_vendor_quad_price = event_hotel.vendor_quad_price,
                               total_price = db.ReservationDetails.Where(r => r.reservation_id == res.id).Select(p => p.amount).Sum(),
                               paid_amount = res.paid_amount,
                               total_amount_after_tax = res.total_amount_after_tax,
                               total_amount_from_vendor = res.total_amount_from_vendor,
                               advance_reservation_percentage = res.advance_reservation_percentage,
                               tax_amount = res.tax_amount,
                               is_canceled = res.is_canceled
                               //profit = calculateProfit(res.id).profit

                           }).Where(r=>r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled == null);
            //Where(r=>r.status == (int)PaymentStatus.Paid).ToList();
            var reservation = db.Reservations;
            double? collected = 0.0;
            double? balance = 0.0;
            double? profit = 0.0;
            int? total_nights = 0;
            if (reservation != null)
            {
                collected = db.Reservations.Where(t=>t.is_canceled == null).Select(t => t.paid_amount).Sum();
                balance = db.Reservations.Where(t => t.is_canceled == null).Select(t => t.total_amount_after_tax).Sum() - collected;
                profit = resData.ToList().Select(t => t.total_amount_after_tax).Sum() - resData.ToList().Select(t => t.total_amount_from_vendor).Sum();
                total_nights = db.Reservations.Where(t => t.is_canceled == null).Select(n => n.total_nights).Sum();
            }
            var displayResult = resData.OrderByDescending(u => u.id).Skip(skip)
                 .Take(pageSize).ToList();
            var totalRecords = resData.Count();

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = displayResult,
                collected = collected,
                balance = balance,
                profit = profit,
                total_nights = total_nights

            }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult canceledReservations()
        //{
        //    var draw = Request.Form.GetValues("draw").FirstOrDefault();
        //    var start = Request.Form.GetValues("start").FirstOrDefault();
        //    var length = Request.Form.GetValues("length").FirstOrDefault();
        //    var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
        //    var from_date = Request.Form.GetValues("columns[0][search][value]")[0];
        //    var to_date = Request.Form.GetValues("columns[1][search][value]")[0];
        //    int pageSize = length != null ? Convert.ToInt32(length) : 0;
        //    int skip = start != null ? Convert.ToInt32(start) : 0;
        //    var resData = (from res in db.Reservations
        //                   join company in db.Companies on res.company_id equals company.id
        //                   join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
        //                   join even in db.Events on event_hotel.event_id equals even.id
        //                   join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
        //                   join oU in db.Users on res.opener equals oU.id into us
        //                   from opener in us.DefaultIfEmpty()
        //                   join cU in db.Users on res.closer equals cU.id into use
        //                   from closer in use.DefaultIfEmpty()
        //                   select new ReservationViewModel
        //                   {
        //                       id = res.id,
        //                       total_amount = res.total_amount,
        //                       currency = res.currency,
        //                       tax = res.tax,
        //                       financial_advance = res.financial_advance,
        //                       financial_advance_date = res.financial_advance_date,
        //                       financial_due = res.financial_due,
        //                       financial_due_date = res.financial_due_date,
        //                       financial_due_date_string = res.financial_due_date.ToString(),
        //                       status = res.status,
        //                       single_price = res.single_price,
        //                       double_price = res.double_price,
        //                       triple_price = res.triple_price,
        //                       quad_price = res.quad_price,
        //                       vendor_single_price = res.vendor_single_price,
        //                       vendor_douple_price = res.vendor_douple_price,
        //                       vendor_triple_price = res.vendor_triple_price,
        //                       vendor_quad_price = res.vendor_quad_price,
        //                       vendor_id = res.vendor_id,
        //                       active = res.active,
        //                       company_name = company.name,
        //                       phone = company.phone,
        //                       email = company.email,
        //                       company_id = res.company_id,
        //                       event_hotel_id = event_hotel.id,
        //                       hotel_name = hotel.name,
        //                       hotel_rate = hotel.rate,
        //                       reservations_officer_name = res.reservations_officer_name,
        //                       reservations_officer_email = res.reservations_officer_email,
        //                       reservations_officer_phone = res.reservations_officer_phone,
        //                       opener = res.opener,
        //                       closer = res.closer,
        //                       opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
        //                       closer_name = opener.full_name == null ? "No Closer Assigned" : closer.full_name,
        //                       check_in = res.check_in,
        //                       check_out = res.check_out,
        //                       string_check_in = res.check_in.ToString(),
        //                       string_check_out = res.check_out.ToString(),
        //                       total_nights = res.total_nights,
        //                       total_rooms = res.total_rooms,
        //                       profit = res.profit,
        //                       shift = res.shift,
        //                       event_name = even.title,
        //                       event_tax = even.tax,
        //                       event_single_price = event_hotel.single_price,
        //                       event_double_price = event_hotel.double_price,
        //                       event_triple_price = event_hotel.triple_price,
        //                       event_quad_price = event_hotel.quad_price,
        //                       event_vendor_single_price = event_hotel.vendor_single_price,
        //                       event_vendor_double_price = event_hotel.vendor_douple_price,
        //                       event_vendor_triple_price = event_hotel.vendor_triple_price,
        //                       event_vendor_quad_price = event_hotel.vendor_quad_price,
        //                       total_price = db.ReservationDetails.Where(r => r.reservation_id == res.id).Select(p => p.amount).Sum(),
        //                       paid_amount = res.paid_amount,
        //                       total_amount_after_tax = res.total_amount_after_tax,
        //                       total_amount_from_vendor = res.total_amount_from_vendor,
        //                       advance_reservation_percentage = res.advance_reservation_percentage,
        //                       tax_amount = res.tax_amount,
        //                       is_canceled = res.is_canceled
        //                       //profit = calculateProfit(res.id).profit

        //                   }).Where(r => r.is_canceled == 1);
        //    //Where(r=>r.status == (int)PaymentStatus.Paid).ToList();
        //    var reservation = db.Reservations;
        //    double? collected = 0.0;
        //    double? balance = 0.0;
        //    double? profit = 0.0;
        //    int? total_nights = 0;
        //    if (reservation != null)
        //    {
        //        collected = db.Reservations.Select(t => t.paid_amount).Sum();
        //        balance = db.Reservations.Select(t => t.total_amount_after_tax).Sum() - collected;
        //        profit = resData.ToList().Select(t => t.total_amount_after_tax).Sum() - resData.ToList().Select(t => t.total_amount_from_vendor).Sum();
        //        total_nights = db.Reservations.Select(n => n.total_nights).Sum();
        //    }
        //    var displayResult = resData.OrderByDescending(u => u.id).Skip(skip)
        //         .Take(pageSize).ToList();
        //    var totalRecords = resData.Count();

        //    return Json(new
        //    {
        //        draw = draw,
        //        recordsTotal = totalRecords,
        //        recordsFiltered = totalRecords,
        //        data = displayResult,
        //        collected = collected,
        //        balance = balance,
        //        profit = profit,
        //        total_nights = total_nights

        //    }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult canceledReservations()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var from_date = Request.Form.GetValues("columns[0][search][value]")[0];
            var to_date = Request.Form.GetValues("columns[1][search][value]")[0];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var resData = (from resDet in db.ReservationDetails
                           join client in db.Clients on resDet.client_id equals client.id
                           join res in db.Reservations on resDet.reservation_id equals res.id
                           join company in db.Companies on res.company_id equals company.id
                           join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                           join even in db.Events on event_hotel.event_id equals even.id
                           join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                           select new ReservationDetailViewModel
                           {
                               id = resDet.id,
                               event_name = even.title,
                               hotel_name = hotel.name,
                               reservation_from = resDet.reservation_from,
                               reservation_to = resDet.reservation_to,
                               string_reservation_from = resDet.reservation_from.ToString(),
                               string_reservation_to = resDet.reservation_to.ToString(),
                               price_per_night = (resDet.amount_after_tax / resDet.no_of_days),
                               paid_to_vendor = resDet.paid_to_vendor,
                               client_first_name = client.first_name,
                               client_last_name = client.last_name,
                               no_of_days = resDet.no_of_days,
                               is_canceled = res.is_canceled

                           }).Where(r => r.paid_to_vendor != 1 && r.is_canceled == 1);

            var displayResult = resData.OrderByDescending(u => u.id).Skip(skip)
                 .Take(pageSize).ToList();
            var totalRecords = resData.Count();

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = displayResult
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ourRooms()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var from_date = Request.Form.GetValues("columns[0][search][value]")[0];
            var to_date = Request.Form.GetValues("columns[1][search][value]")[0];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var resData = (from resDet in db.ReservationDetails 
                           join client in db.Clients on resDet.client_id equals client.id
                           join res in db.Reservations on resDet.reservation_id equals res.id
                           join company in db.Companies on res.company_id equals company.id
                           join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                           join even in db.Events on event_hotel.event_id equals even.id
                           join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                           select new ReservationDetailViewModel
                           {
                               id = resDet.id,
                               event_name = even.title,
                               hotel_name = hotel.name,
                               reservation_from = resDet.reservation_from,
                               reservation_to = resDet.reservation_to,
                               string_reservation_from = resDet.reservation_from.ToString(),
                               string_reservation_to = resDet.reservation_to.ToString(),
                               price_per_night = (resDet.amount_after_tax / resDet.no_of_days),
                               paid_to_vendor = resDet.paid_to_vendor,
                               client_first_name = client.first_name,
                               client_last_name = client.last_name,
                               no_of_days = resDet.no_of_days,
                               is_canceled = res.is_canceled

                           }).Where(r => r.paid_to_vendor == 1 && r.is_canceled == 1);
           
            var displayResult = resData.OrderByDescending(u => u.id).Skip(skip)
                 .Take(pageSize).ToList();
            var totalRecords = resData.Count();

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = displayResult
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult sendMail(string email)
        {
            MailMessage mail =
                 new MailMessage(
                     "hazem.marawan@hotmail.com",
                     "prog.hassan.sayed@gmail.com",
                     "Agency Confimation",
                     "Hello Mr: Hassan"
                     );

            SmtpClient client = new SmtpClient("smtp.live.com", 587);
            client.UseDefaultCredentials = true;

            NetworkCredential credentials = new NetworkCredential("hazem.marawan@hotmail.com", "Elpop12345%");

            client.Credentials = credentials;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.Send(mail);

            return Json(new { email = email }, JsonRequestBehavior.AllowGet);
        }

        public void ExportPartiallyPaidReservations()
        {
            User currentUser = Session["user"] as User;

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Available Assets");

            System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#000000");
            Sheet.Cells["A1:F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            Sheet.Cells["A1:F1"].Style.Fill.BackgroundColor.SetColor(colFromHex);
            System.Drawing.Color text = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            Sheet.Cells["A1:F1"].Style.Font.Color.SetColor(text);

            Sheet.Cells["A1"].Value = "#";
            Sheet.Cells["B1"].Value = "Company INFO";
            Sheet.Cells["C1"].Value = "Total Amount";
            Sheet.Cells["D1"].Value = "Financial Info";
            Sheet.Cells["E1"].Value = "Hotel Info";
            Sheet.Cells["F1"].Value = "Created At";

            List<ReservationViewModel> reservations = (from res in db.Reservations
                           join company in db.Companies on res.company_id equals company.id
                           join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                           join even in db.Events on event_hotel.event_id equals even.id
                           join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id

                           select new ReservationViewModel
                           {
                               id = res.id,
                               total_amount = res.total_amount,
                               currency = res.currency,
                               tax = res.tax,
                               financial_advance = res.financial_advance,
                               financial_advance_date = res.financial_advance_date,
                               financial_due = res.financial_due,
                               financial_due_date = res.financial_due_date,
                               status = res.status,
                               single_price = res.single_price,
                               double_price = res.double_price,
                               active = res.active,
                               company_name = company.name,
                               phone = company.phone,
                               email = company.email,
                               event_hotel_id = event_hotel.id,
                               hotel_name = hotel.name,
                               hotel_rate = hotel.rate,
                               reservations_officer_name = res.reservations_officer_name,
                               reservations_officer_email = res.reservations_officer_email,
                               reservations_officer_phone = res.reservations_officer_phone,
                               paid_amount = res.financial_advance + res.financial_due

                           }).Where(r => r.status == (int)PaymentStatus.Partially).ToList();

            int row = 2;
            foreach (var reservation in reservations)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = reservation.id;
                Sheet.Cells[string.Format("B{0}", row)].Value = "Name: "+reservation.company_name+"-"+ "Officer  Name: " + reservation.reservations_officer_name+"-"+ "Email: " + reservation.email +"-" + "Phone: " + reservation.phone;
                Sheet.Cells[string.Format("C{0}", row)].Value = "Total Amount: "+reservation.total_amount+"-"+"Tax: "+reservation.tax;
                Sheet.Cells[string.Format("D{0}", row)].Value = "Advance: "+reservation.financial_advance+"-"+"Single Price: "+reservation.single_price+"-"+"Double Price: "+reservation.double_price + "-"+"Status: "+(reservation.status==1?"Partially Paid":"Paid");
                Sheet.Cells[string.Format("E{0}", row)].Value = "Hotel: "+reservation.hotel_name+"-"+"Rate: "+reservation.hotel_rate;
                Sheet.Cells[string.Format("F{0}", row)].Value = reservation.created_at.ToString();
                row++;
            }

            row++;
            colFromHex = System.Drawing.ColorTranslator.FromHtml("#000000");
            Sheet.Cells[string.Format("A{0},B{1}", row, row)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            Sheet.Cells[string.Format("A{0},B{1}", row, row)].Style.Fill.BackgroundColor.SetColor(colFromHex);
            text = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            Sheet.Cells[string.Format("A{0},B{1}", row, row)].Style.Font.Color.SetColor(text);

            Sheet.Cells[string.Format("A{0}", row)].Value = "Total";
            Sheet.Cells[string.Format("B{0}", row)].Value = reservations.Count();

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        public void ExportPaidReservations()
        {
            User currentUser = Session["user"] as User;

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Available Assets");

            System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#000000");
            Sheet.Cells["A1:F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            Sheet.Cells["A1:F1"].Style.Fill.BackgroundColor.SetColor(colFromHex);
            System.Drawing.Color text = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            Sheet.Cells["A1:F1"].Style.Font.Color.SetColor(text);

            Sheet.Cells["A1"].Value = "#";
            Sheet.Cells["B1"].Value = "Company INFO";
            Sheet.Cells["C1"].Value = "Total Amount";
            Sheet.Cells["D1"].Value = "Financial Info";
            Sheet.Cells["E1"].Value = "Hotel Info";
            Sheet.Cells["F1"].Value = "Created At";

            List<ReservationViewModel> reservations = (from res in db.Reservations
                                                       join company in db.Companies on res.company_id equals company.id
                                                       join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                                                       join even in db.Events on event_hotel.event_id equals even.id
                                                       join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id

                                                       select new ReservationViewModel
                                                       {
                                                           id = res.id,
                                                           total_amount = res.total_amount,
                                                           currency = res.currency,
                                                           tax = res.tax,
                                                           financial_advance = res.financial_advance,
                                                           financial_advance_date = res.financial_advance_date,
                                                           financial_due = res.financial_due,
                                                           financial_due_date = res.financial_due_date,
                                                           status = res.status,
                                                           single_price = res.single_price,
                                                           double_price = res.double_price,
                                                           active = res.active,
                                                           company_name = company.name,
                                                           phone = company.phone,
                                                           email = company.email,
                                                           event_hotel_id = event_hotel.id,
                                                           hotel_name = hotel.name,
                                                           hotel_rate = hotel.rate,
                                                           reservations_officer_name = res.reservations_officer_name,
                                                           reservations_officer_email = res.reservations_officer_email,
                                                           reservations_officer_phone = res.reservations_officer_phone,
                                                           paid_amount = res.financial_advance + res.financial_due

                                                       }).Where(r => r.status == (int)PaymentStatus.Paid).ToList();

            int row = 2;
            foreach (var reservation in reservations)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = reservation.id;
                Sheet.Cells[string.Format("B{0}", row)].Value = "Name: " + reservation.company_name + "-" + "Officer  Name: " + reservation.reservations_officer_name + "-" + "Email: " + reservation.email + "-" + "Phone: " + reservation.phone;
                Sheet.Cells[string.Format("C{0}", row)].Value = "Total Amount: " + reservation.total_amount + "-" + "Tax: " + reservation.tax;
                Sheet.Cells[string.Format("D{0}", row)].Value = "Advance: " + reservation.financial_advance + "-" + "Single Price: " + reservation.single_price + "-" + "Double Price: " + reservation.double_price + "-" + "Status: " + (reservation.status == 1 ? "Partially Paid" : "Paid");
                Sheet.Cells[string.Format("E{0}", row)].Value = "Hotel: " + reservation.hotel_name + "-" + "Rate: " + reservation.hotel_rate;
                Sheet.Cells[string.Format("F{0}", row)].Value = reservation.created_at.ToString();
                row++;
            }

            row++;
            colFromHex = System.Drawing.ColorTranslator.FromHtml("#000000");
            Sheet.Cells[string.Format("A{0},B{1}", row, row)].Style.Fill.PatternType = ExcelFillStyle.Solid;
            Sheet.Cells[string.Format("A{0},B{1}", row, row)].Style.Fill.BackgroundColor.SetColor(colFromHex);
            text = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            Sheet.Cells[string.Format("A{0},B{1}", row, row)].Style.Font.Color.SetColor(text);

            Sheet.Cells[string.Format("A{0}", row)].Value = "Total";
            Sheet.Cells[string.Format("B{0}", row)].Value = reservations.Count();

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult saveReservation(ReservationViewModel reservationViewModel)
        {
            if (reservationViewModel.id == 0)
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
                reservation.balance_due_date = DateTime.Now;
                reservation.active = 1;
                reservation.created_by = Session["id"].ToString().ToInt();
                db.Reservations.Add(reservation);
                db.SaveChanges();

                InitialReservation initialReservation = AutoMapper.Mapper.Map<Reservation, InitialReservation>(reservation);
                initialReservation.id = 0;
                initialReservation.reservation_id = reservation.id;
                db.InitialReservations.Add(initialReservation);
                db.SaveChanges();

                Company company = new Company();
                company.name = reservationViewModel.company_name;
                company.created_at = DateTime.Now;
                company.updated_at = DateTime.Now;
                company.created_by = Session["id"].ToString().ToInt();
                db.Companies.Add(company);
                db.SaveChanges();

                reservation.company_id = company.id;
                db.SaveChanges();

                Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservation.id, "Add", "Add Booking #" + reservation.id);

            } else
            {
                Reservation reservation = db.Reservations.Find(reservationViewModel.id);
                reservation.reservations_officer_name = reservationViewModel.reservations_officer_name;
                reservation.reservations_officer_email = reservationViewModel.reservations_officer_email;
                reservation.reservations_officer_phone = reservationViewModel.reservations_officer_phone;
                reservation.shift = reservationViewModel.shift;
                reservation.opener = reservationViewModel.opener;
                reservation.closer = reservationViewModel.closer;
                reservation.single_price = reservationViewModel.single_price;
                reservation.vendor_single_price = reservationViewModel.vendor_single_price;
                reservation.double_price = reservationViewModel.double_price;
                reservation.vendor_douple_price = reservationViewModel.vendor_douple_price;
                reservation.triple_price = reservationViewModel.triple_price;
                reservation.vendor_triple_price = reservationViewModel.vendor_triple_price;
                reservation.quad_price = reservationViewModel.quad_price;
                reservation.vendor_quad_price = reservationViewModel.vendor_quad_price;
                reservation.vendor_id = reservationViewModel.vendor_id;
                reservation.tax = reservationViewModel.tax;
                reservation.currency = reservationViewModel.currency;
                reservation.advance_reservation_percentage = reservationViewModel.advance_reservation_percentage;
                reservation.active = 1;
                reservation.updated_at = DateTime.Now;
                reservation.updated_by = Session["id"].ToString().ToInt();

                List<ReservationDetail> reservationDetails = db.ReservationDetails.Where(r => r.reservation_id == reservation.id).ToList();
                if (reservationDetails.Count() != 0)
                {
                    //Hazem
                    DateTime minDate = reservationDetails.Select(s => s.reservation_from).Min();
                    DateTime maxDate = reservationDetails.Select(s => s.reservation_to).Max();

                    reservation.check_in = minDate;
                    reservation.check_out = maxDate;
                }

                ReservationViewModel updatedTotals = ReservationService.calculateTotalandVendor(reservation.id);

                reservation.total_amount = updatedTotals.total_amount;
                reservation.total_amount_after_tax = updatedTotals.total_amount_after_tax;
                reservation.total_amount_from_vendor = updatedTotals.total_amount_from_vendor;
                reservation.tax_amount = updatedTotals.tax_amount;
                reservation.total_nights = updatedTotals.total_nights;
                reservation.active = 1;
                reservation.updated_at = DateTime.Now;
                reservation.updated_by = Session["id"].ToString().ToInt();
                db.Entry(reservation).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                Company company = db.Companies.Find(reservation.company_id);
                company.name = reservationViewModel.company_name;
                db.Entry(company).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservation.id, "Edit", "Edit Booking #" + reservation.id);


            }
            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getEventHotels(int id)
        {
            List<EventHotelViewModel> eventHotelViewModel = (from evH in db.EventHotels
                                                       join hotel in db.Hotels on evH.hotel_id equals hotel.id
                                                       select new EventHotelViewModel
                                                       {
                                                           id = evH.id,
                                                           name = hotel.name,
                                                           event_id = evH.event_id
                                                           
                                                       }).Where(r => r.event_id == id).ToList();

            return Json(new { hotels = eventHotelViewModel }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult cancelReservation(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            reservation.is_canceled = 1;
            db.SaveChanges();

            Logs.ReservationActionLog(Session["id"].ToString().ToInt(), id, "Cancel", "Cancel Reservation #" + id);

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }
    }
}