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
            ViewBag.Hotels = db.Hotels.Select(s => new { s.id, s.name }).ToList();

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
                           join vend in db.Vendors on event_hotel.vendor_id equals vend.id into ven
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
                               is_special = even.is_special,
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
                               total_rooms = res.total_rooms,
                               profit = res.profit,
                               shift = res.shift,
                               created_at_string = res.created_at.ToString(),
                               updated_at_string = res.updated_at.ToString(),
                               updated_by = res.updated_by,
                               event_name = even.title,
                               event_tax = even.tax,
                               reservation_avg_price = res.reservation_avg_price,
                               vendor_avg_price = res.vendor_avg_price,
                               event_single_price = event_hotel.single_price,
                               event_double_price = event_hotel.double_price,
                               event_triple_price = event_hotel.triple_price,
                               event_quad_price = event_hotel.quad_price,
                               event_vendor_single_price = event_hotel.vendor_single_price,
                               event_vendor_double_price = event_hotel.vendor_douple_price,
                               event_vendor_triple_price = event_hotel.vendor_triple_price,
                               event_vendor_quad_price = event_hotel.vendor_quad_price,
                               paid_amount = res.paid_amount,
                               total_amount_after_tax = res.total_amount_after_tax,
                               total_amount_from_vendor = res.total_amount_from_vendor,
                               advance_reservation_percentage = res.advance_reservation_percentage,
                               tax_amount = res.tax_amount,
                               created_by_name = createdBy.full_name,
                               updated_by_name = updatedBy.full_name == null ? "Not Updated" : updatedBy.full_name,
                               shift_name = ((Shift)res.shift).ToString(),
                               location_name = loc.name,
                               city_name = city.name,
                               vendor_code = vendor.code,
                               card_expiration_date = res.card_expiration_date,
                               credit_card_number = res.credit_card_number,
                               security_code = res.security_code,
                               reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel {
                                   id = resCre.id,
                                   security_code = resCre.security_code,
                                   credit_card_number = resCre.credit_card_number,
                                   card_expiration_date = resCre.card_expiration_date
                               }).ToList()
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
                               is_special = even.is_special,
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
                               total_rooms = reinitialRes.total_rooms,
                               profit = reinitialRes.profit,
                               shift = reinitialRes.shift,
                               created_at_string = reinitialRes.created_at.ToString(),
                               updated_at_string = reinitialRes.updated_at.ToString(),
                               updated_by = reinitialRes.updated_by,
                               event_name = even.title,
                               event_tax = even.tax,
                               reservation_avg_price = reinitialRes.reservation_avg_price,
                               vendor_avg_price = reinitialRes.vendor_avg_price,
                               event_single_price = event_hotel.single_price,
                               event_double_price = event_hotel.double_price,
                               event_triple_price = event_hotel.triple_price,
                               event_quad_price = event_hotel.quad_price,
                               event_vendor_single_price = event_hotel.vendor_single_price,
                               event_vendor_double_price = event_hotel.vendor_douple_price,
                               event_vendor_triple_price = event_hotel.vendor_triple_price,
                               event_vendor_quad_price = event_hotel.vendor_quad_price,
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
            if(initialResData == null)
            {
                initialResData = new ReservationViewModel();
            }
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

        public JsonResult websiteReservations()
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
                               string_currency = res.currency != null? ((Currency)res.currency).ToString(): ((Currency)0).ToString(),
                               tax = res.tax,
                               is_special = even.is_special,
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
                               created_by = res.created_by,
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
                               is_canceled = res.is_canceled,
                               is_refund = res.is_refund
                               //profit = calculateProfit(res.id).profit
                           }).Where(r => r.paid_amount == 0 && r.is_canceled == null && r.created_by == null);
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
                               string_currency = res.currency != null ? ((Currency)res.currency).ToString() : ((Currency)0).ToString(),
                               tax = res.tax,
                               is_special = even.is_special,
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
                               created_by = res.created_by,
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
                               is_canceled = res.is_canceled,
                               is_refund = res.is_refund
                               //profit = calculateProfit(res.id).profit
                           }).Where(r => r.paid_amount == 0 && r.is_canceled == null && r.created_by != null);
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
                               string_currency = res.currency != null ? ((Currency)res.currency).ToString() : ((Currency)0).ToString(),
                               tax = res.tax,
                               financial_advance = res.financial_advance,
                               financial_advance_date = res.financial_advance_date,
                               financial_due = res.financial_due,
                               financial_due_date = res.financial_due_date,
                               status = res.status,
                               is_special = even.is_special,
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
                               is_canceled = res.is_canceled,
                               is_refund = res.is_refund
                               //profit = calculateProfit(res.id).profit

                           }).Where(r=>r.paid_amount < r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled == null && r.is_refund == 0);
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
                               is_special = even.is_special,
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
                               is_canceled = res.is_canceled,
                               is_refund = res.is_refund
                               //profit = calculateProfit(res.id).profit

                           }).Where(r=>r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled == null && r.is_refund == 0);
            //Where(r=>r.status == (int)PaymentStatus.Paid).ToList();
            var reservation = db.Reservations;
            double? collected = 0.0;
            double? balance = 0.0;
            double? profit = 0.0;
            double? refund = 0.0;
            int? total_nights = 0;
            if (reservation != null)
            {
                collected = db.Reservations.Where(t=>t.is_canceled == null).Select(t => t.paid_amount).Sum();
                balance = db.Reservations.Where(t => t.is_canceled == null).Select(t => t.total_amount_after_tax).Sum() - collected;
                profit = resData.ToList().Where(r => r.is_refund == 0).Select(t => t.total_amount_after_tax).Sum() - resData.ToList().Select(t => t.total_amount_from_vendor).Sum();
                refund = db.Reservations.Where(r => r.is_refund == 1).Select(t => t.paid_amount).Sum();
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
                total_nights = total_nights,
                refund = refund
            }, JsonRequestBehavior.AllowGet);
        }

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
                               is_special = even.is_special,
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
                               is_canceled = res.is_canceled,
                               //profit = calculateProfit(res.id).profit
                               is_refund = res.is_refund
                           }).Where(r => r.is_canceled == 1 && r.is_refund == 0);
            //Where(r => r.status == (int)PaymentStatus.Paid).ToList();
            var reservation = db.Reservations;
            double? collected = 0.0;
            double? balance = 0.0;
            double? profit = 0.0;
            int? total_nights = 0;
            if (reservation != null)
            {
                collected = db.Reservations.Select(t => t.paid_amount).Sum();
                balance = db.Reservations.Select(t => t.total_amount_after_tax).Sum() - collected;
                profit = resData.ToList().Select(t => t.total_amount_after_tax).Sum() - resData.ToList().Select(t => t.total_amount_from_vendor).Sum();
                total_nights = db.Reservations.Select(n => n.total_nights).Sum();
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

        public JsonResult canceledReservationsOLD()
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
        public JsonResult ourRoomsOLD()
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
                               is_special = even.is_special,
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
                               is_canceled = res.is_canceled,
                               is_refund = res.is_refund,
                               countPaidToVendorRooms = db.ReservationDetails.Where(r => r.paid_to_vendor == 1 && r.reservation_id == res.id).Count(),
                               countPaidToVendorNights = db.ReservationDetails.Where(r => r.paid_to_vendor == 1 && r.reservation_id == res.id).Sum(n => n.no_of_days),
                               //profit = calculateProfit(res.id).profit
                           }).Where(r => r.is_canceled == 1&& r.countPaidToVendorRooms > 0);
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

        public JsonResult refund()
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
                               is_special = even.is_special,
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
                               is_canceled = res.is_canceled,
                               is_refund = res.is_refund,
                               countPaidToVendorRooms = db.ReservationDetails.Where(r => r.paid_to_vendor == 1 && r.reservation_id == res.id).Count(),
                               countPaidToVendorNights = db.ReservationDetails.Where(r => r.paid_to_vendor == 1 && r.reservation_id == res.id).Sum(n => n.no_of_days),
                               //profit = calculateProfit(res.id).profit
                           }).Where(r => r.is_refund == 1);
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


        public ActionResult ViewOurRooms(int id)
        {
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
                                   is_canceled = res.is_canceled,
                                   reservation_id = res.id,
                                   vendor_code = resDet.vendor_code,
                                   cancelation_policy = resDet.cancelation_policy,
                                   amount = resDet.amount_after_tax,
                                   room_type = resDet.room_type,
                               }).Where(r => r.paid_to_vendor == 1 && r.is_canceled == 1 && r.reservation_id == id);

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

            return View();
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
                if(reservationViewModel.is_special == 1)
                {
                    Event specialEvent = new Event();
                    specialEvent.is_special = 1;
                    specialEvent.event_from = DateTime.Now;
                    specialEvent.to = DateTime.Now;
                    specialEvent.due_date = DateTime.Now;
                    specialEvent.created_at = DateTime.Now;
                    specialEvent.created_by = Session["id"].ToString().ToInt(); 
                    db.Events.Add(specialEvent);
                    db.SaveChanges();

                    EventHotel specialEventHotel = new EventHotel();
                    specialEventHotel.event_id = specialEvent.id;
                    specialEventHotel.hotel_id = reservationViewModel.hotel_id;
                    specialEventHotel.created_at = DateTime.Now;
                    specialEventHotel.created_by = Session["id"].ToString().ToInt();
                    db.EventHotels.Add(specialEventHotel);
                    db.SaveChanges();

                    Reservation reservation = AutoMapper.Mapper.Map<ReservationViewModel, Reservation>(reservationViewModel);
                    reservation.event_hotel_id = specialEventHotel.id;
                    reservation.single_price = 0;
                    reservation.vendor_single_price = 0;
                    reservation.double_price = 0;
                    reservation.vendor_douple_price = 0;
                    reservation.triple_price = 0;
                    reservation.vendor_triple_price = 0;
                    reservation.quad_price = 0;
                    reservation.vendor_quad_price = 0;
                    reservation.tax = 0;
                    reservation.currency = 0;
                    reservation.tax_amount = 0;
                    reservation.total_amount_after_tax = 0;
                    reservation.total_amount_from_vendor = 0;
                    reservation.total_amount = 0;
                    reservation.paid_amount = 0;
                    reservation.financial_advance = 0;
                    reservation.financial_due = 0;
                    reservation.advance_reservation_percentage = 0;
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


                }
                else
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

                }

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

        public ActionResult balanceDueDate()
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
                var balanceDueDateReservations = (from res in db.Reservations
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
                                                      is_canceled = res.is_canceled,
                                                      balance_due_date = (DateTime)res.balance_due_date,
                                                      countPaidToVendorRooms = db.ReservationDetails.Where(resDet => resDet.reservation_id == res.id).Where(resDet => resDet.paid_to_vendor == 1).Count()
                                                      //profit = calculateProfit(res.id).profit
                                                      //}).Where(r => r.balance_due_date.Year == DateTime.Now.Year && r.balance_due_date.Month == DateTime.Now.Month && r.balance_due_date.Day == DateTime.Now.Day && r.is_canceled == null && r.total_amount_after_tax != 0 && r.paid_amount < r.total_amount_after_tax).ToList();
                                                  }).Where(r => r.balance_due_date <= DateTime.Now && r.is_canceled == null && r.total_amount_after_tax != 0 && r.paid_amount < r.total_amount_after_tax);

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    balanceDueDateReservations = balanceDueDateReservations.Where(m => m.company_name.ToLower().Contains(searchValue.ToLower()) || m.id.ToString().ToLower().Contains(searchValue.ToLower()) ||
                     m.hotel_name.ToLower().Contains(searchValue.ToLower()));
                }

                //total number of rows count     
                var displayResult = balanceDueDateReservations.OrderByDescending(u => u.id).Skip(skip)
                     .Take(pageSize).ToList();
                var totalRecords = balanceDueDateReservations.Count();

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
        public ActionResult PayToVendor()
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

                var payToVendorReservations = (from resDetail in db.ReservationDetails
                                                                            join res in db.Reservations on resDetail.reservation_id equals res.id
                                                                            join company in db.Companies on res.company_id equals company.id
                                                                            join client in db.Clients on resDetail.client_id equals client.id
                                                                            join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                                                                            select new ReservationDetailViewModel
                                                                            {
                                                                                id = resDetail.id,
                                                                                amount_after_tax = resDetail.amount_after_tax,
                                                                                tax = resDetail.tax,
                                                                                room_type = resDetail.room_type,
                                                                                reservation_from = resDetail.reservation_from,
                                                                                reservation_to = resDetail.reservation_to,
                                                                                no_of_days = resDetail.no_of_days,
                                                                                active = resDetail.active,
                                                                                client_id = resDetail.client_id,
                                                                                client_first_name = client.first_name,
                                                                                client_last_name = client.last_name,
                                                                                company_name = company.name,
                                                                                string_currency = ((Currency)res.currency).ToString(),
                                                                                reservation_id = resDetail.reservation_id,
                                                                                currency = res.currency,
                                                                                is_reservation_canceled = res.is_canceled,
                                                                                string_reservation_from = resDetail.reservation_from.ToString(),
                                                                                string_reservation_to = resDetail.reservation_to.ToString(),
                                                                                vendor_code = resDetail.vendor_code,
                                                                                vendor_cost = resDetail.vendor_cost,
                                                                                notify = resDetail.notify,
                                                                                is_canceled = resDetail.is_canceled,
                                                                                paid_to_vendor = resDetail.paid_to_vendor,
                                                                                payment_to_vendor_deadline = resDetail.payment_to_vendor_deadline,
                                                                                payment_to_vendor_notification_date = resDetail.payment_to_vendor_notification_date,
                                                                                paid_to_vendor_date = resDetail.paid_to_vendor_date,
                                                                                amount_paid_to_vendor = resDetail.amount_paid_to_vendor,
                                                                                cancelation_policy = resDetail.cancelation_policy,
                                                                                confirmation_id = resDetail.confirmation_id,

                                                                            }).Where(s => s.paid_to_vendor != 1 && s.is_reservation_canceled != 1 && (s.payment_to_vendor_deadline <= DateTime.Now || s.payment_to_vendor_notification_date <= DateTime.Now));
                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    payToVendorReservations = payToVendorReservations.Where(m =>
                     m.client_first_name.ToLower().Contains(searchValue.ToLower()) 
                     ||m.id.ToString().ToLower().Contains(searchValue.ToLower()) 
                     ||m.client_last_name.ToLower().Contains(searchValue.ToLower())
                     || m.company_name.ToString().ToLower().Contains(searchValue.ToLower())
                     );
                }

                //total number of rows count     
                var displayResult = payToVendorReservations.OrderByDescending(u => u.id).Skip(skip)
                     .Take(pageSize).ToList();
                var totalRecords = payToVendorReservations.Count();

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

        public JsonResult paidReservationToVendor(ReservationDetailViewModel reservationDetailViewModel)
        {
            ReservationDetail reservationDetail = db.ReservationDetails.Find(reservationDetailViewModel.id);
            reservationDetail.paid_to_vendor = reservationDetailViewModel.paid_to_vendor;
            db.SaveChanges();

            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult payAmountToReservation(ReservationViewModel reservationViewModel)
        {
            Reservation reservation = db.Reservations.Find(reservationViewModel.id);
            reservation.paid_amount += reservationViewModel.paid_amount;
            db.SaveChanges();

            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult refundReservation(ReservationViewModel reservationViewModel)
        {
            Reservation reservation = db.Reservations.Find(reservationViewModel.id);
            reservation.is_refund = 1;
            db.SaveChanges();

            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }
    }
}