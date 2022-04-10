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
using System.IO;

namespace Agency.Controllers
{
    [CustomAuthenticationFilter]
    public class ReservationController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();

        // GET: Reservation
        public ActionResult Index()
        {
            ViewBag.Events = db.Events.Where(s => s.is_special != 1).Select(s => new { s.id, s.title, s.updated_at }).OrderByDescending(e => e.updated_at).ToList();
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
                               credit = res.credit,
                               refund = res.refund,
                               company_id = res.company_id,
                               event_hotel_id = event_hotel.id,
                               hotel_name = res.hotel_name,
                               hotel_rate = hotel.rate,
                               reservations_officer_name = res.reservations_officer_name,
                               reservations_officer_email = res.reservations_officer_email,
                               reservations_officer_phone = res.reservations_officer_phone,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "-" : opener.full_name,
                               closer_name = closer.full_name == null ? "-" : closer.full_name,
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
                               reservation_avg_price_before_tax = res.reservation_avg_price_before_tax,
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
                               reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel
                               {
                                   id = resCre.id,
                                   security_code = resCre.security_code,
                                   credit_card_number = resCre.credit_card_number,
                                   card_expiration_date = resCre.card_expiration_date
                               }).ToList(),
                               refund_id = res.refund_id,
                               hotel_name_special = res.hotel_name,
                               transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                               {
                                   id = tr.id,
                                   reservation_id = tr.reservation_id,
                                   amount = tr.amount,
                                   transaction_id = tr.transaction_id
                               }).ToList(),
                               cancelation_fees = res.cancelation_fees
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
                                      hotel_name = reinitialRes.hotel_name,
                                      hotel_rate = hotel.rate,
                                      reservations_officer_name = reinitialRes.reservations_officer_name,
                                      reservations_officer_email = reinitialRes.reservations_officer_email,
                                      reservations_officer_phone = reinitialRes.reservations_officer_phone,
                                      opener = reinitialRes.opener,
                                      closer = reinitialRes.closer,
                                      opener_name = opener.full_name == null ? "-" : opener.full_name,
                                      closer_name = closer.full_name == null ? "-" : closer.full_name,
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
                                      reservation_avg_price_before_tax = reinitialRes.reservation_avg_price_before_tax,
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
                                      security_code = reinitialRes.security_code,
                                      credit = resData.credit,
                                      hotel_name_special = resData.hotel_name,
                                      transactions = db.Transactions.Where(t => t.reservation_id == resData.id).Select(tr => new TransactionViewModel
                                      {
                                          id = tr.id,
                                          reservation_id = tr.reservation_id,
                                          amount = tr.amount,
                                          transaction_id = tr.transaction_id
                                      }).ToList(),
                                      cancelation_fees = resData.cancelation_fees
                                  }).Where(r => r.reservation_id == id).FirstOrDefault();

            if (initialResData == null)
            {
                initialResData = new ReservationViewModel();
            }

            ReservationViewScreenViewModel reservationViewScreenViewModel = new ReservationViewScreenViewModel();
            reservationViewScreenViewModel.CurrentReservation = resData;
            reservationViewScreenViewModel.InitialReservation = initialResData;

            ViewBag.id = id;
            ViewBag.Users = db.Users.Select(s => new { s.id, s.full_name }).ToList();
            ViewBag.Events = db.Events.Where(s => s.is_special != 1).Select(s => new { s.id, s.title, s.updated_at }).OrderByDescending(e => e.updated_at).ToList();

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
            foreach (var resDetail in reservationDetails)
            {

                double? room_price = 0;
                double? vendor_room_price = 0;
                if (resDetail.room_type == 1)
                {
                    room_price = reservation.single_price > 0 ? reservation.single_price : eventHotel.single_price;
                    vendor_room_price = reservation.vendor_single_price > 0 ? reservation.vendor_single_price : eventHotel.vendor_single_price;
                }
                else if (resDetail.room_type == 2)
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

                total_amount += ((room_price * resDetail.no_of_days) + ((room_price * resDetail.no_of_days) * (reservation.tax / 100)));
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
                           join c in db.Companies on res.company_id equals c.id into co
                           from company in co.DefaultIfEmpty()
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
                               reservations_officer_email_2 = res.reservations_officer_email_2,
                               reservations_officer_phone_2 = res.reservations_officer_phone_2,
                               reservations_officer_email_3 = res.reservations_officer_email_3,
                               reservations_officer_phone_3 = res.reservations_officer_phone_3,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = closer.full_name == null ? "No Closer Assigned" : closer.full_name,
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
                               created_at = res.created_at,
                               reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel
                               {
                                   id = resCre.id,
                                   security_code = resCre.security_code,
                                   credit_card_number = resCre.credit_card_number,
                                   card_expiration_date = resCre.card_expiration_date
                               }).ToList(),
                               hotel_name_special = res.hotel_name,
                               transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                               {
                                   id = tr.id,
                                   reservation_id = tr.reservation_id,
                                   amount = tr.amount,
                                   transaction_id = tr.transaction_id
                               }).ToList(),
                               cancelation_fees = res.cancelation_fees
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
                           join c in db.Companies on res.company_id equals c.id into co
                           from company in co.DefaultIfEmpty()
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
                               reservations_officer_email_2 = res.reservations_officer_email_2,
                               reservations_officer_phone_2 = res.reservations_officer_phone_2,
                               reservations_officer_email_3 = res.reservations_officer_email_3,
                               reservations_officer_phone_3 = res.reservations_officer_phone_3,
                               created_by = res.created_by,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = closer.full_name == null ? "No Closer Assigned" : closer.full_name,
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
                               created_at = res.created_at,
                               reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel
                               {
                                   id = resCre.id,
                                   security_code = resCre.security_code,
                                   credit_card_number = resCre.credit_card_number,
                                   card_expiration_date = resCre.card_expiration_date
                               }).ToList(),
                               hotel_name_special = res.hotel_name,
                               transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                               {
                                   id = tr.id,
                                   reservation_id = tr.reservation_id,
                                   amount = tr.amount,
                                   transaction_id = tr.transaction_id
                               }).ToList(),
                               cancelation_fees = res.cancelation_fees
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

        public JsonResult checkoutReservations()
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
                           join c in db.Companies on res.company_id equals c.id into co
                           from company in co.DefaultIfEmpty()
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
                               reservations_officer_email_2 = res.reservations_officer_email_2,
                               reservations_officer_phone_2 = res.reservations_officer_phone_2,
                               reservations_officer_email_3 = res.reservations_officer_email_3,
                               reservations_officer_phone_3 = res.reservations_officer_phone_3,
                               created_by = res.created_by,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = closer.full_name == null ? "No Closer Assigned" : closer.full_name,
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
                               refund = res.refund,
                               total_amount_after_tax = res.total_amount_after_tax,
                               total_amount_from_vendor = res.total_amount_from_vendor,
                               advance_reservation_percentage = res.advance_reservation_percentage,
                               tax_amount = res.tax_amount,
                               is_canceled = res.is_canceled,
                               is_refund = res.is_refund,
                               created_at = res.created_at,
                               transaction_fees = db.Transactions.Where(r => r.reservation_id == res.id).Select(s=>s.amount).Sum(),
                               reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel
                               {
                                   id = resCre.id,
                                   security_code = resCre.security_code,
                                   credit_card_number = resCre.credit_card_number,
                                   card_expiration_date = resCre.card_expiration_date
                               }).ToList(),
                               hotel_name_special = res.hotel_name,
                               transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                               {
                                   id = tr.id,
                                   reservation_id = tr.reservation_id,
                                   amount = tr.amount,
                                   transaction_id = tr.transaction_id
                               }).ToList(),
                               cancelation_fees = res.cancelation_fees
                               //profit = calculateProfit(res.id).profit
                           }).Where(r => r.check_out <= DateTime.Now && r.is_canceled == null);

            if (!string.IsNullOrEmpty(from_date))
            {
                if (Convert.ToDateTime(from_date) != DateTime.MinValue)
                {
                    DateTime from = Convert.ToDateTime(from_date);
                    resData = resData.Where(s => s.check_out >= from);
                }
            }

            if (!string.IsNullOrEmpty(to_date))
            {
                if (Convert.ToDateTime(to_date) != DateTime.MinValue)
                {
                    DateTime to = Convert.ToDateTime(to_date);
                    resData = resData.Where(s => s.check_out <= to);
                }
            }

            //start Statistics

            StatisticsViewModel statisticsViewModel = new StatisticsViewModel();
            var reservation = resData;
            List<ReservationViewModel> reservationList = resData.ToList();
            if (reservationList.Count() > 0)
            {

                statisticsViewModel.total_amount_after_tax_sum = reservation.Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum();
                statisticsViewModel.collected = reservation.Where(t => t.is_canceled != 1).Select(t => t.paid_amount).Sum();
                statisticsViewModel.collected_percentage = (statisticsViewModel.collected / statisticsViewModel.total_amount_after_tax_sum) * 100;

                statisticsViewModel.balance = reservation.Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum() - statisticsViewModel.collected;
                statisticsViewModel.balance_percentage = (statisticsViewModel.balance / statisticsViewModel.total_amount_after_tax_sum) * 100;

                double? reservation_cancelation_fees = 0.0;
                List<ReservationViewModel> reservations = reservation.Where(c => c.is_canceled == 1 && c.cancelation_fees != null).ToList();

                if (reservations.Count() > 0)
                {
                    reservation_cancelation_fees = reservation.Where(c => c.is_canceled == 1 && c.cancelation_fees != null).Select(c => c.cancelation_fees).Sum();
                }

                statisticsViewModel.transaction_fees = reservation.Select(tf => tf.transaction_fees).Sum();

                statisticsViewModel.profit = reservation.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum() - db.Reservations.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Where(t => t.is_canceled != 1).Select(t => t.total_amount_from_vendor).Sum();
                statisticsViewModel.profit += reservation_cancelation_fees;
                statisticsViewModel.profit -= statisticsViewModel.transaction_fees;

                statisticsViewModel.total_amount_after_tax_sum_for_profit = reservation.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum();
                statisticsViewModel.profit_percentage = (statisticsViewModel.profit / statisticsViewModel.total_amount_after_tax_sum_for_profit) * 100;
                
                statisticsViewModel.refund = 0;
                List<Double?> reservationsRefund = reservation.Where(t => t.is_canceled != 1).Select(t => t.refund).ToList();
                if (reservationsRefund.Count() > 0)
                {
                    statisticsViewModel.refund = reservation.Where(t => t.is_canceled != 1).Select(t => t.refund).Sum();
                }
                statisticsViewModel.refund_percentage = (statisticsViewModel.refund / statisticsViewModel.total_amount_after_tax_sum) * 100;

                statisticsViewModel.total_nights = reservation.Where(t => t.is_canceled != 1).Select(n => n.total_nights).Sum();
            } else
            {
                statisticsViewModel.collected = 0;
                statisticsViewModel.balance = 0;
                statisticsViewModel.profit = 0;
                statisticsViewModel.total_nights = 0;
                statisticsViewModel.refund = 0;
            }

            //end Statistics

            var displayResult = resData.OrderByDescending(u => u.id).Skip(skip)
                 .Take(pageSize).ToList();
            var totalRecords = resData.Count();

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = displayResult,
                collected = statisticsViewModel.collected,
                balance = statisticsViewModel.balance,
                profit = statisticsViewModel.profit,
                total_nights = statisticsViewModel.total_nights,
                refund = statisticsViewModel.refund
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
                           join c in db.Companies on res.company_id equals c.id into co
                           from company in co.DefaultIfEmpty()
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
                               reservations_officer_email_2 = res.reservations_officer_email_2,
                               reservations_officer_phone_2 = res.reservations_officer_phone_2,
                               reservations_officer_email_3 = res.reservations_officer_email_3,
                               reservations_officer_phone_3 = res.reservations_officer_phone_3,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = closer.full_name == null ? "No Closer Assigned" : closer.full_name,
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
                               is_refund = res.is_refund,
                               created_at = res.created_at,
                               reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel
                               {
                                   id = resCre.id,
                                   security_code = resCre.security_code,
                                   credit_card_number = resCre.credit_card_number,
                                   card_expiration_date = resCre.card_expiration_date
                               }).ToList(),
                               hotel_name_special = res.hotel_name,
                               transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                               {
                                   id = tr.id,
                                   reservation_id = tr.reservation_id,
                                   amount = tr.amount,
                                   transaction_id = tr.transaction_id
                               }).ToList(),
                               cancelation_fees = res.cancelation_fees
                               //profit = calculateProfit(res.id).profit

                           }).Where(r => r.paid_amount < r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled == null);
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
                           join c in db.Companies on res.company_id equals c.id into co
                           from company in co.DefaultIfEmpty()
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
                               reservations_officer_email_2 = res.reservations_officer_email_2,
                               reservations_officer_phone_2 = res.reservations_officer_phone_2,
                               reservations_officer_email_3 = res.reservations_officer_email_3,
                               reservations_officer_phone_3 = res.reservations_officer_phone_3,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = closer.full_name == null ? "No Closer Assigned" : closer.full_name,
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
                               is_refund = res.is_refund,
                               created_at = res.created_at,
                               reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel
                               {
                                   id = resCre.id,
                                   security_code = resCre.security_code,
                                   credit_card_number = resCre.credit_card_number,
                                   card_expiration_date = resCre.card_expiration_date
                               }).ToList(),
                               hotel_name_special = res.hotel_name,
                               transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                               {
                                   id = tr.id,
                                   reservation_id = tr.reservation_id,
                                   amount = tr.amount,
                                   transaction_id = tr.transaction_id
                               }).ToList(),
                               cancelation_fees = res.cancelation_fees
                               //profit = calculateProfit(res.id).profit

                           }).Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled == null);
            //Where(r=>r.status == (int)PaymentStatus.Paid).ToList();

            StatisticsViewModel statisticsViewModel = ReservationService.calculateStatistics();
            var displayResult = resData.OrderByDescending(u => u.id).Skip(skip)
                 .Take(pageSize).ToList();
            var totalRecords = resData.Count();

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = displayResult,
                collected = statisticsViewModel.collected,
                balance = statisticsViewModel.balance,
                profit = statisticsViewModel.profit,
                total_nights = statisticsViewModel.total_nights,
                refund = statisticsViewModel.refund
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
                           join c in db.Companies on res.company_id equals c.id into co
                           from company in co.DefaultIfEmpty()
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
                               reservations_officer_email_2 = res.reservations_officer_email_2,
                               reservations_officer_phone_2 = res.reservations_officer_phone_2,
                               reservations_officer_email_3 = res.reservations_officer_email_3,
                               reservations_officer_phone_3 = res.reservations_officer_phone_3,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = closer.full_name == null ? "No Closer Assigned" : closer.full_name,
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
                               is_refund = res.is_refund,
                               created_at = res.created_at,
                               reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel
                               {
                                   id = resCre.id,
                                   security_code = resCre.security_code,
                                   credit_card_number = resCre.credit_card_number,
                                   card_expiration_date = resCre.card_expiration_date
                               }).ToList(),
                               hotel_name_special = res.hotel_name,
                               transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                               {
                                   id = tr.id,
                                   reservation_id = tr.reservation_id,
                                   amount = tr.amount,
                                   transaction_id = tr.transaction_id
                               }).ToList(),
                               cancelation_fees = res.cancelation_fees
                           }).Where(r => r.is_canceled == 1);
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
                           join cli in db.Clients on resDet.client_id equals cli.id into cl
                           from client in cl.DefaultIfEmpty()
                           join re in db.Reservations on resDet.reservation_id equals re.id into ress
                           from res in ress.DefaultIfEmpty()
                           join c in db.Companies on res.company_id equals c.id into co
                           from company in co.DefaultIfEmpty()
                           join eve_hot in db.EventHotels on res.event_hotel_id equals eve_hot.id into ev_ho
                           from event_hotel in ev_ho.DefaultIfEmpty()
                           join ev in db.Events on event_hotel.event_id equals ev.id into eve
                           from even in eve.DefaultIfEmpty()
                           join hot in db.Hotels on event_hotel.hotel_id equals hot.id into hotl
                           from hotel in hotl.DefaultIfEmpty()
                           select new ReservationDetailViewModel
                           {
                               id = resDet.id,
                               reservation_id = resDet.reservation_id,
                               event_name = even.title,
                               additional_show_name = resDet.additional_show_name,
                               additional_hotel_name = resDet.additional_hotel_name,
                               hotel_name = res.hotel_name,
                               reservation_from = resDet.reservation_from,
                               reservation_to = resDet.reservation_to,
                               string_reservation_from = resDet.reservation_from.ToString(),
                               string_reservation_to = resDet.reservation_to.ToString(),
                               price_per_night = (resDet.amount_after_tax / resDet.no_of_days),
                               paid_to_vendor = resDet.paid_to_vendor,
                               client_first_name = client.first_name,
                               client_last_name = client.last_name,
                               client_id = client.id,
                               room_type = resDet.room_type,
                               vendor_code = resDet.vendor_code,
                               vendor_cost = resDet.vendor_cost,
                               no_of_days = resDet.no_of_days,
                               is_canceled = res.is_canceled,
                               payment_to_vendor_deadline = resDet.payment_to_vendor_deadline

                           }).Where(r => r.is_canceled == 1 || r.reservation_id == null);

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

        public JsonResult all()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var from_date_check_in = Request.Form.GetValues("columns[0][search][value]")[0];
            var to_date_check_in = Request.Form.GetValues("columns[1][search][value]")[0];
            var from_date_check_out = Request.Form.GetValues("columns[2][search][value]")[0];
            var to_date_check_out = Request.Form.GetValues("columns[3][search][value]")[0];
            var hotel_id = Request.Form.GetValues("columns[4][search][value]")[0];
            var event_id = Request.Form.GetValues("columns[5][search][value]")[0];
            var is_special = Request.Form.GetValues("columns[6][search][value]")[0];
            var from_balance_due_date = Request.Form.GetValues("columns[7][search][value]")[0];
            var to_balance_due_date = Request.Form.GetValues("columns[8][search][value]")[0];
            var search_opener = Request.Form.GetValues("columns[9][search][value]")[0];
            var search_closer = Request.Form.GetValues("columns[10][search][value]")[0];
            var search_status = Request.Form.GetValues("columns[11][search][value]")[0];
            var from_created_at = Request.Form.GetValues("columns[12][search][value]")[0];
            var to_created_at = Request.Form.GetValues("columns[13][search][value]")[0];
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            var resData = (from res in db.Reservations
                           join c in db.Companies on res.company_id equals c.id into co
                           from company in co.DefaultIfEmpty()
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
                               created_by = res.created_by,
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
                               hotel_id = hotel.id,
                               hotel_name = hotel.name,
                               hotel_rate = hotel.rate,
                               reservations_officer_name = res.reservations_officer_name,
                               reservations_officer_email = res.reservations_officer_email,
                               reservations_officer_phone = res.reservations_officer_phone,
                               reservations_officer_email_2 = res.reservations_officer_email_2,
                               reservations_officer_phone_2 = res.reservations_officer_phone_2,
                               reservations_officer_email_3 = res.reservations_officer_email_3,
                               reservations_officer_phone_3 = res.reservations_officer_phone_3,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = closer.full_name == null ? "No Closer Assigned" : closer.full_name,
                               check_in = res.check_in,
                               check_out = res.check_out,
                               string_check_in = res.check_in.ToString(),
                               string_check_out = res.check_out.ToString(),
                               total_nights = res.total_nights,
                               total_rooms = res.total_rooms,
                               profit = res.profit,
                               shift = res.shift,
                               event_id = even.id,
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
                               refund = res.refund,
                               created_at = res.created_at,
                               reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel
                               {
                                   id = resCre.id,
                                   security_code = resCre.security_code,
                                   credit_card_number = resCre.credit_card_number,
                                   card_expiration_date = resCre.card_expiration_date
                               }).ToList(),
                               hotel_name_special = res.hotel_name,
                               transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                               {
                                   id = tr.id,
                                   reservation_id = tr.reservation_id,
                                   amount = tr.amount,
                                   transaction_id = tr.transaction_id
                               }).ToList(),
                               cancelation_fees = res.cancelation_fees
                               //profit = calculateProfit(res.id).profit

                           });
            //Where(r=>r.status == (int)PaymentStatus.Paid).ToList();

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                resData = resData.Where(m => m.company_name.ToLower().Contains(searchValue.ToLower()) || m.id.ToString().ToLower().Contains(searchValue.ToLower()) ||
                 m.opener_name.ToLower().Contains(searchValue.ToLower()) || m.closer_name.ToLower().Contains(searchValue.ToLower()));
            }

            if (!string.IsNullOrEmpty(from_date_check_in))
            {
                if (Convert.ToDateTime(from_date_check_in) != DateTime.MinValue)
                {
                    DateTime from = Convert.ToDateTime(from_date_check_in);
                    resData = resData.Where(s => s.check_in >= from);
                }
            }

            if (!string.IsNullOrEmpty(to_date_check_in))
            {
                if (Convert.ToDateTime(to_date_check_in) != DateTime.MinValue)
                {
                    DateTime to = Convert.ToDateTime(to_date_check_in);
                    resData = resData.Where(s => s.check_out <= to);
                }
            }

            if (!string.IsNullOrEmpty(from_date_check_out))
            {
                if (Convert.ToDateTime(from_date_check_out) != DateTime.MinValue)
                {
                    DateTime from = Convert.ToDateTime(from_date_check_out);
                    resData = resData.Where(s => s.check_in >= from);
                }
            }

            if (!string.IsNullOrEmpty(to_date_check_out))
            {
                if (Convert.ToDateTime(to_date_check_out) != DateTime.MinValue)
                {
                    DateTime to = Convert.ToDateTime(to_date_check_out);
                    resData = resData.Where(s => s.check_out <= to);
                }
            }

            if (!string.IsNullOrEmpty(hotel_id))
            {
                int hotel_id_int = int.Parse(hotel_id);
                resData = resData.Where(s => s.hotel_id == hotel_id_int);
            }

            if (!string.IsNullOrEmpty(event_id))
            {
                int event_id_int = int.Parse(event_id);
                resData = resData.Where(s => s.event_id == event_id_int);
            }

            if (!string.IsNullOrEmpty(is_special))
            {
                int is_special_int = int.Parse(is_special);
                resData = resData.Where(s => s.is_special == is_special_int);
            }

            if (!string.IsNullOrEmpty(from_balance_due_date))
            {
                if (Convert.ToDateTime(from_balance_due_date) != DateTime.MinValue)
                {
                    DateTime from = Convert.ToDateTime(from_balance_due_date);
                    resData = resData.Where(s => s.check_in >= from);
                }
            }

            if (!string.IsNullOrEmpty(to_balance_due_date))
            {
                if (Convert.ToDateTime(to_balance_due_date) != DateTime.MinValue)
                {
                    DateTime to = Convert.ToDateTime(to_balance_due_date);
                    resData = resData.Where(s => s.check_out <= to);
                }
            }

            if (!string.IsNullOrEmpty(search_opener))
            {
                int opener = int.Parse(search_opener);
                resData = resData.Where(s => s.opener == opener);
            }

            if (!string.IsNullOrEmpty(search_closer))
            {
                int closer = int.Parse(search_closer);
                resData = resData.Where(s => s.is_special == closer);
            }

            if (!string.IsNullOrEmpty(search_status))
            {
                int status = int.Parse(search_status);
                if (status == 1)
                {
                    resData = resData.Where(s => s.created_by == null);

                }
                else if (status == 2)
                {
                    resData = resData.Where(s => s.paid_amount == 0);

                }
                else if (status == 3)
                {
                    resData = resData.Where(s => s.paid_amount != 0 && s.paid_amount < s.total_amount_after_tax);

                }
                else if (status == 4)
                {
                    resData = resData.Where(s => s.paid_amount != 0 && s.paid_amount == s.total_amount_after_tax);

                }
                else if (status == 5)
                {
                    resData = resData.Where(s => s.is_canceled == 1);

                }
                else if (status == 6)
                {
                    resData = resData.Where(s => s.paid_amount != null && s.paid_amount != 0);

                }
            }


            if (!string.IsNullOrEmpty(from_created_at))
            {
                if (Convert.ToDateTime(from_created_at) != DateTime.MinValue)
                {
                    DateTime from = Convert.ToDateTime(from_created_at);
                    resData = resData.Where(s => s.created_at >= from);
                }
            }

            if (!string.IsNullOrEmpty(to_created_at))
            {
                if (Convert.ToDateTime(to_created_at) != DateTime.MinValue)
                {
                    DateTime to = Convert.ToDateTime(to_created_at);
                    resData = resData.Where(s => s.created_at <= to);
                }
            }


            //start Statistics
            StatisticsViewModel statisticsViewModel = new StatisticsViewModel();
            var reservation = resData;
            List<ReservationViewModel> reservationList = resData.ToList();
            if (reservationList.Count() > 0)
            {
                //statisticsViewModel.transaction_fees = reservation.Where(t => t.is_canceled != 1).Select(tf => tf.transactions).Sum();

                statisticsViewModel.total_amount_after_tax_sum = reservation.Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum();
                statisticsViewModel.collected = reservation.Where(t => t.is_canceled != 1).Select(t => t.paid_amount).Sum();
                statisticsViewModel.collected_percentage = (statisticsViewModel.collected / statisticsViewModel.total_amount_after_tax_sum) * 100;

                statisticsViewModel.balance = reservation.Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum() - statisticsViewModel.collected;
                statisticsViewModel.balance_percentage = (statisticsViewModel.balance / statisticsViewModel.total_amount_after_tax_sum) * 100;

                double? reservation_cancelation_fees = 0.0;
                List<ReservationViewModel> reservations = reservation.Where(c => c.is_canceled == 1 && c.cancelation_fees != null).ToList();

                if (reservations.Count() > 0)
                {
                    reservation_cancelation_fees = reservation.Where(c => c.is_canceled == 1 && c.cancelation_fees != null).Select(c => c.cancelation_fees).Sum();
                }

                statisticsViewModel.profit = reservation.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum() - db.Reservations.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Where(t => t.is_canceled != 1).Select(t => t.total_amount_from_vendor).Sum();
                statisticsViewModel.profit += reservation_cancelation_fees;

                statisticsViewModel.total_amount_after_tax_sum_for_profit = reservation.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum();
                statisticsViewModel.profit_percentage = (statisticsViewModel.profit / statisticsViewModel.total_amount_after_tax_sum_for_profit) * 100;

                statisticsViewModel.refund = 0;
                List<Double?> reservationsRefund = reservation.Where(t => t.is_canceled != 1).Select(t => t.refund).ToList();
                if (reservationsRefund.Count() > 0)
                {
                    statisticsViewModel.refund = reservation.Where(t => t.is_canceled != 1).Select(t => t.refund).Sum();
                }
                statisticsViewModel.refund_percentage = (statisticsViewModel.refund / statisticsViewModel.total_amount_after_tax_sum) * 100;

                statisticsViewModel.total_nights = reservation.Where(t => t.is_canceled != 1).Select(n => n.total_nights).Sum();
            }
            else
            {
                statisticsViewModel.collected = 0;
                statisticsViewModel.balance = 0;
                statisticsViewModel.profit = 0;
                statisticsViewModel.total_nights = 0;
                statisticsViewModel.refund = 0;
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
                collected = statisticsViewModel.collected,
                balance = statisticsViewModel.balance,
                profit = statisticsViewModel.profit,
                total_nights = statisticsViewModel.total_nights,
                refund = statisticsViewModel.refund
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
                               reservations_officer_email_2 = res.reservations_officer_email_2,
                               reservations_officer_phone_2 = res.reservations_officer_phone_2,
                               reservations_officer_email_3 = res.reservations_officer_email_3,
                               reservations_officer_phone_3 = res.reservations_officer_phone_3,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = closer.full_name == null ? "No Closer Assigned" : closer.full_name,
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
                               created_at = res.created_at,
                               countPaidToVendorRooms = db.ReservationDetails.Where(r => r.paid_to_vendor == 1 && r.reservation_id == res.id).Count(),
                               countPaidToVendorNights = db.ReservationDetails.Where(r => r.paid_to_vendor == 1 && r.reservation_id == res.id).Sum(n => n.no_of_days),
                               hotel_name_special = res.hotel_name,
                               transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                               {
                                   id = tr.id,
                                   reservation_id = tr.reservation_id,
                                   amount = tr.amount,
                                   transaction_id = tr.transaction_id
                               }).ToList(),
                               cancelation_fees = res.cancelation_fees
                               //profit = calculateProfit(res.id).profit
                           }).Where(r => r.is_refund == 1);

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

                if (reservationViewModel.is_special == 1)
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

                    Hotel hotel = db.Hotels.Find(reservationViewModel.hotel_id);

                    Reservation reservation = AutoMapper.Mapper.Map<ReservationViewModel, Reservation>(reservationViewModel);

                    reservation.hotel_name = reservationViewModel.hotel_name;
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
                    reservation.total_nights = ((DateTime)reservationViewModel.check_out - (DateTime)reservationViewModel.check_in).Days;
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

                    for (int i = 1; i <= reservationViewModel.total_rooms; i++)
                    {
                        ReservationDetail reservationDetail = new ReservationDetail();
                        reservationDetail.reservation_id = reservation.id;
                        reservationDetail.reservation_from = (DateTime)reservationViewModel.check_in;
                        reservationDetail.reservation_to = (DateTime)reservationViewModel.check_out;
                        reservationDetail.no_of_days = reservation.total_nights + 1;
                        reservationDetail.room_price = 0;
                        reservationDetail.amount = 0;
                        reservationDetail.tax = 0;
                        reservationDetail.amount_after_tax = 0;
                        db.ReservationDetails.Add(reservationDetail);
                        db.SaveChanges();

                        reservationDetail.parent_id = reservationDetail.id;
                        db.SaveChanges();
                    }
                    Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservation.id, "Add", "Add Booking #" + reservation.id);


                }
                else
                {
                    Reservation reservation = AutoMapper.Mapper.Map<ReservationViewModel, Reservation>(reservationViewModel);
                    EventHotel eventHotel = db.EventHotels.Find(reservation.event_hotel_id);
                    Event c_event = db.Events.Find(eventHotel.event_id);

                    Hotel hotel = db.Hotels.Find(eventHotel.hotel_id);

                    reservation.hotel_name = hotel.name;
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

                    for (int i = 1; i <= reservationViewModel.total_rooms; i++)
                    {
                        ReservationDetail reservationDetail = new ReservationDetail();
                        reservationDetail.reservation_id = reservation.id;
                        reservationDetail.reservation_from = (DateTime)reservationViewModel.check_in;
                        reservationDetail.reservation_to = (DateTime)reservationViewModel.check_out;
                        reservationDetail.no_of_days = ((DateTime)reservationViewModel.check_out - (DateTime)reservationViewModel.check_in).Days + 1;
                        reservationDetail.room_type = 1;
                        reservationDetail.room_price = reservation.single_price;
                        reservationDetail.amount = reservation.single_price * (reservationDetail.no_of_days - 1);
                        reservationDetail.tax = reservationDetail.amount * (reservation.tax / 100);
                        reservationDetail.amount_after_tax = reservationDetail.tax + reservationDetail.amount;
                        db.ReservationDetails.Add(reservationDetail);
                        db.SaveChanges();

                        reservationDetail.parent_id = reservationDetail.id;
                        db.SaveChanges();

                    }
                    Reservation currentReservation = ReservationService.UpdateTotals(reservation.id);
                    InitialReservation initialReservation = AutoMapper.Mapper.Map<Reservation, InitialReservation>(currentReservation);
                    initialReservation.id = 0;
                    initialReservation.reservation_id = reservation.id;
                    db.InitialReservations.Add(initialReservation);
                    db.SaveChanges();
                }

            }
            else
            {
                Reservation reservation = db.Reservations.Find(reservationViewModel.id);

                string editHtml = "";
                if (reservation.hotel_name != reservationViewModel.hotel_name)
                    editHtml += "Hotel Name: "+ reservation.hotel_name + @" | <span style = ""color:red;"">" + reservationViewModel.hotel_name + "</span><br>";
                reservation.hotel_name = reservationViewModel.hotel_name;

                if (reservation.reservations_officer_name != reservationViewModel.reservations_officer_name)
                    editHtml += "Officer Name: " + reservation.reservations_officer_name + @" | <span style = ""color:red;"">" + reservationViewModel.reservations_officer_name + "</span><br>";
                reservation.reservations_officer_name = reservationViewModel.reservations_officer_name;

                if (reservation.reservations_officer_email != reservationViewModel.reservations_officer_email)
                    editHtml += "Officer Email: " + reservation.reservations_officer_email + @" | <span style = ""color:red;"">" + reservationViewModel.reservations_officer_email + "</span><br>";
                reservation.reservations_officer_email = reservationViewModel.reservations_officer_email;

                if (reservation.reservations_officer_phone != reservationViewModel.reservations_officer_phone)
                    editHtml += "Officer Phone: " + reservation.reservations_officer_phone + @" | <span style = ""color:red;"">" + reservationViewModel.reservations_officer_phone + "</span><br>";
                reservation.reservations_officer_phone = reservationViewModel.reservations_officer_phone;

                if (reservation.reservations_officer_email_2 != reservationViewModel.reservations_officer_email_2)
                    editHtml += "Officer Email2: " + reservation.reservations_officer_email_2 + @" | <span style = ""color:red;"">" + reservationViewModel.reservations_officer_email_2 + "</span><br>";
                reservation.reservations_officer_email_2 = reservationViewModel.reservations_officer_email_2;

                if (reservation.reservations_officer_phone_2 != reservationViewModel.reservations_officer_phone_2)
                    editHtml += "Officer Phone2: " + reservation.reservations_officer_phone_2 + @" | <span style = ""color:red;"">" + reservationViewModel.reservations_officer_phone_2 + "</span><br>";
                reservation.reservations_officer_phone_2 = reservationViewModel.reservations_officer_phone_2;

                if (reservation.reservations_officer_email_3 != reservationViewModel.reservations_officer_email_3)
                    editHtml += "Officer Email3: " + reservation.reservations_officer_email_3 + @" | <span style = ""color:red;"">" + reservationViewModel.reservations_officer_email_3 + "</span><br>";
                reservation.reservations_officer_email_3 = reservationViewModel.reservations_officer_email_3;

                if (reservation.reservations_officer_phone_3 != reservationViewModel.reservations_officer_phone_3)
                    editHtml += "Officer Phone3: " + reservation.reservations_officer_phone_3 + @" | <span style = ""color:red;"">" + reservationViewModel.reservations_officer_phone_3 + "</span><br>";
                reservation.reservations_officer_phone_3 = reservationViewModel.reservations_officer_phone_3;

                if (reservation.shift != reservationViewModel.shift)
                    editHtml += "Shift: " + ((Shift)reservation.shift).ToString() + @" | <span style = ""color:red;"">" + ((Shift)reservationViewModel.shift).ToString() + "</span><br>";
                reservation.shift = reservationViewModel.shift;

                if (reservation.opener != reservationViewModel.opener)
                    editHtml += "Opener: " + reservation.opener + @" | <span style = ""color:red;"">" + reservationViewModel.opener + "</span><br>";
                reservation.opener = reservationViewModel.opener;

                if (reservation.closer != reservationViewModel.closer)
                    editHtml += "Closer: " + reservation.closer + @" | <span style = ""color:red;"">" + reservationViewModel.closer + "</span><br>";
                reservation.closer = reservationViewModel.closer;

                if (reservation.single_price != reservationViewModel.single_price)
                    editHtml += "Single Price: " + reservation.single_price + @" | <span style = ""color:red;"">" + reservationViewModel.single_price + "</span><br>";
                reservation.single_price = reservationViewModel.single_price;

                if (reservation.vendor_single_price != reservationViewModel.vendor_single_price)
                    editHtml += "Vendor Single Price: " + reservation.vendor_single_price + @" | <span style = ""color:red;"">" + reservationViewModel.vendor_single_price + "</span><br>";
                reservation.vendor_single_price = reservationViewModel.vendor_single_price;

                if (reservation.double_price != reservationViewModel.double_price)
                    editHtml += "Double Price: " + reservation.double_price + @" | <span style = ""color:red;"">" + reservationViewModel.double_price + "</span><br>";
                reservation.double_price = reservationViewModel.double_price;

                if (reservation.vendor_douple_price != reservationViewModel.vendor_douple_price)
                    editHtml += "Vendor Douple Price: " + reservation.vendor_douple_price + @" | <span style = ""color:red;"">" + reservationViewModel.vendor_douple_price + "</span><br>";
                reservation.vendor_douple_price = reservationViewModel.vendor_douple_price;

                if (reservation.triple_price != reservationViewModel.triple_price)
                    editHtml += "Triple Price: " + reservation.triple_price + @" | <span style = ""color:red;"">" + reservationViewModel.triple_price + "</span><br>";
                reservation.triple_price = reservationViewModel.triple_price;

                if (reservation.vendor_triple_price != reservationViewModel.vendor_triple_price)
                    editHtml += "Vendor Triple Price: " + reservation.vendor_triple_price + @" | <span style = ""color:red;"">" + reservationViewModel.vendor_triple_price + "</span><br>";
                reservation.vendor_triple_price = reservationViewModel.vendor_triple_price;

                if (reservation.quad_price != reservationViewModel.quad_price)
                    editHtml += "Quad Price: " + reservation.quad_price + @" | <span style = ""color:red;"">" + reservationViewModel.quad_price + "</span><br>";
                reservation.quad_price = reservationViewModel.quad_price;

                if (reservation.vendor_quad_price != reservationViewModel.vendor_quad_price)
                    editHtml += "Vendor Quad Price: " + reservation.vendor_quad_price + @" | <span style = ""color:red;"">" + reservationViewModel.vendor_quad_price + "</span><br>";
                reservation.vendor_quad_price = reservationViewModel.vendor_quad_price;

                reservation.vendor_id = reservationViewModel.vendor_id;

                if (reservation.tax != reservationViewModel.tax)
                    editHtml += "Tax: " + reservation.tax + @" | <span style = ""color:red;"">" + reservationViewModel.tax + "</span><br>";
                reservation.tax = reservationViewModel.tax;

                if (reservation.currency != reservationViewModel.currency)
                    editHtml += "Currency: " + reservation.currency + @" | <span style = ""color:red;"">" + reservationViewModel.currency + "</span><br>";
                reservation.currency = reservationViewModel.currency;

                if (reservation.advance_reservation_percentage != reservationViewModel.advance_reservation_percentage)
                    editHtml += "Reservation Percentage: " + reservation.advance_reservation_percentage + @" | <span style = ""color:red;"">" + reservationViewModel.advance_reservation_percentage + "</span><br>";
                reservation.advance_reservation_percentage = reservationViewModel.advance_reservation_percentage;

                reservation.active = 1;

                editHtml += "Updated: " + reservation.updated_at.ToString().Split(' ')[0] + @" | <span style = ""color:red;"">" + DateTime.Now.ToString().Split(' ')[0] + "</span><br>";
                reservation.updated_at = DateTime.Now;

                reservation.updated_by = Session["id"].ToString().ToInt();

                List<ReservationDetail> reservationDetails = db.ReservationDetails.Where(r => r.reservation_id == reservation.id).ToList();
                if (reservationDetails.Count() != 0)
                {
                    
                    DateTime minDate = reservationDetails.Select(s => s.reservation_from).Min();
                    DateTime maxDate = reservationDetails.Select(s => s.reservation_to).Max();

                    reservation.check_in = minDate;
                    reservation.check_out = maxDate;
                }

                ReservationService.UpdateTotals(reservation.id);

                if (reservation.company_id != null)
                {
                    Company company = db.Companies.Find(reservation.company_id);
                    company.name = reservationViewModel.company_name;
                    db.Entry(company).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    Company company = new Company();
                    company.name = reservationViewModel.company_name;

                    db.Companies.Add(company);
                    db.SaveChanges();

                    reservation.company_id = company.id;
                    db.SaveChanges();
                }

                //here log
                Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservation.id, "Edit", "Edit Booking #" + reservation.id + "<br/> " + editHtml);

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

        public JsonResult cancelReservation(ReservationViewModel reservationViewModel)
        {
            Reservation reservation = db.Reservations.Find(reservationViewModel.id);
            reservation.is_canceled = 1;
            reservation.cancelation_fees = reservationViewModel.cancelation_fees;
            db.SaveChanges();

            Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservationViewModel.id, "Cancel", "Cancel Reservation #" + reservationViewModel.id);

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
                                                      string_currency = res.currency != null ? ((Currency)res.currency).ToString() : ((Currency)0).ToString(),
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
                                                      hotel_name = res.hotel_name,
                                                      hotel_rate = hotel.rate,
                                                      reservations_officer_name = res.reservations_officer_name,
                                                      reservations_officer_email = res.reservations_officer_email,
                                                      reservations_officer_phone = res.reservations_officer_phone,
                                                      opener = res.opener,
                                                      closer = res.closer,
                                                      opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                                                      closer_name = closer.full_name == null ? "No Closer Assigned" : closer.full_name,
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
                                                      countPaidToVendorRooms = db.ReservationDetails.Where(resDet => resDet.reservation_id == res.id).Where(resDet => resDet.paid_to_vendor == 1).Count(),
                                                      reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel
                                                      {
                                                          id = resCre.id,
                                                          security_code = resCre.security_code,
                                                          credit_card_number = resCre.credit_card_number,
                                                          card_expiration_date = resCre.card_expiration_date
                                                      }).ToList(),
                                                      transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                                                      {
                                                          id = tr.id,
                                                          reservation_id = tr.reservation_id,
                                                          amount = tr.amount,
                                                          transaction_id = tr.transaction_id
                                                      }).ToList(),
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
                     || m.id.ToString().ToLower().Contains(searchValue.ToLower())
                     || m.client_last_name.ToLower().Contains(searchValue.ToLower())
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

            Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservation.id, "Pay", "Paid for Reservation " + reservation.id + " Amount : " + reservationViewModel.paid_amount);

            Transaction transaction = new Transaction();
            transaction.amount = reservationViewModel.paid_amount;
            transaction.reservation_id = reservation.id;
            transaction.transaction_id = reservationViewModel.transaction_id;
            transaction.active = 1;
            transaction.created_at = DateTime.Now;
            transaction.created_by = Session["id"].ToString().ToInt();

            db.Transactions.Add(transaction);
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


        public JsonResult saveCredit(ReservationViewModel reservationViewModel)
        {
            Reservation reservation = db.Reservations.Find(reservationViewModel.id);
            reservation.credit = reservationViewModel.credit;
            db.SaveChanges();

            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult saveRefund(ReservationViewModel reservationViewModel)
        {
            Reservation reservation = db.Reservations.Find(reservationViewModel.id);
            reservation.refund = reservationViewModel.refund;
            reservation.refund_id = reservationViewModel.refund_id;
            db.SaveChanges();

            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult balanceMail(int id)
        {
            ReservationViewModel resData = (from res in db.Reservations
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
                                                balance_due_date = ((DateTime)res.balance_due_date),
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
                                                reservations_officer_email_2 = res.reservations_officer_email_2,
                                                reservations_officer_email_3 = res.reservations_officer_email_3,
                                                reservations_officer_phone = res.reservations_officer_phone,
                                                created_by = res.created_by,
                                                opener = res.opener,
                                                closer = res.closer,
                                                opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                                                closer_name = closer.full_name == null ? "No Closer Assigned" : closer.full_name,
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
                                                created_at = res.created_at,
                                                reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel
                                                {
                                                    id = resCre.id,
                                                    security_code = resCre.security_code,
                                                    credit_card_number = resCre.credit_card_number,
                                                    card_expiration_date = resCre.card_expiration_date
                                                }).ToList(),
                                                hotel_name_special = res.hotel_name,
                                                transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                                                {
                                                    id = tr.id,
                                                    reservation_id = tr.reservation_id,
                                                    amount = tr.amount,
                                                    transaction_id = tr.transaction_id
                                                }).ToList(),
                                                cancelation_fees = res.cancelation_fees
                                            }).Where(r => r.id == id).FirstOrDefault();

            MailServer mailServer = db.MailServers.Where(ms => ms.type == 4).FirstOrDefault();

            resData.welcome_message = mailServer.welcome_message;

            var resDetailData = (from resDetail in db.ReservationDetails
                                 join reserv in db.Reservations on resDetail.reservation_id equals reserv.id into rs
                                 from res in rs.DefaultIfEmpty()
                                 join cli in db.Clients on resDetail.client_id equals cli.id into cl
                                 from client in cl.DefaultIfEmpty()
                                 select new ReservationDetailViewModel
                                 {
                                     id = resDetail.id,
                                     amount = resDetail.amount,
                                     tax = resDetail.tax,
                                     room_type = resDetail.room_type,
                                     reservation_from = db.ReservationDetails.Where(rsd => rsd.parent_id == resDetail.id).Select(rsd => rsd.reservation_from).Min(),
                                     reservation_to = db.ReservationDetails.Where(rsd => rsd.parent_id == resDetail.id).Select(rsd => rsd.reservation_to).Max(),
                                     no_of_days = resDetail.no_of_days,
                                     parent_id = resDetail.parent_id,
                                     active = resDetail.active,
                                     client_id = resDetail.client_id,
                                     client_first_name = client.first_name,
                                     client_last_name = client.last_name,
                                     reservation_id = resDetail.reservation_id,
                                     currency = res.currency,
                                     string_reservation_from = db.ReservationDetails.Where(rsd => rsd.parent_id == resDetail.id).Select(rsd => rsd.reservation_from).Min().ToString(),
                                     string_reservation_to = db.ReservationDetails.Where(rsd => rsd.parent_id == resDetail.id).Select(rsd => rsd.reservation_to).Max().ToString(),
                                     vendor_code = resDetail.vendor_code,
                                     vendor_cost = db.ReservationDetails.Where(rsd => rsd.parent_id == resDetail.id).Select(rsd => rsd.vendor_cost).Sum(),
                                     room_price = resDetail.room_price,
                                     notify = resDetail.notify,
                                     is_transfered = resDetail.is_transfered,
                                     is_canceled = resDetail.is_canceled,
                                     paid_to_vendor = resDetail.paid_to_vendor,
                                     payment_to_vendor_deadline = (DateTime)resDetail.payment_to_vendor_deadline,
                                     string_payment_to_vendor_deadline = resDetail.payment_to_vendor_deadline.ToString(),
                                     string_spayment_to_vendor_notification_date = resDetail.payment_to_vendor_notification_date.ToString(),
                                     payment_to_vendor_notification_date = resDetail.payment_to_vendor_notification_date,
                                     paid_to_vendor_date = resDetail.paid_to_vendor_date,
                                     amount_paid_to_vendor = resDetail.amount_paid_to_vendor,
                                     cancelation_policy = resDetail.cancelation_policy,
                                     confirmation_id = resDetail.confirmation_id,
                                     guestReservations = db.ReservationDetails.Where(r => r.parent_id == resDetail.id && r.id != resDetail.id).Select(guestRes => new AdditionalReservationViewModel
                                     {
                                         id = guestRes.id,
                                         reservation_id = guestRes.reservation_id,
                                         parent_id = guestRes.parent_id,
                                         reservation_from = guestRes.reservation_from,
                                         reservation_to = guestRes.reservation_to,
                                         vendor_code = guestRes.vendor_code,
                                         vendor_cost = guestRes.vendor_cost,
                                         room_price = guestRes.room_price,
                                         string_reservation_from = guestRes.reservation_from.ToString(),
                                         string_reservation_to = guestRes.reservation_to.ToString(),
                                     }).ToList(),
                                 }).Where(d => d.reservation_id == id && d.parent_id == d.id).ToList();

            string balanceMailPath = Path.Combine(Server.MapPath("~/MailTemplates/balance.html"));
            StreamReader streamReader = new StreamReader(balanceMailPath);
            string emailContent = streamReader.ReadToEnd();

            emailContent = emailContent.Replace("_welcome_message", mailServer.welcome_message);
            emailContent = emailContent.Replace("_Name", resData.reservations_officer_name);
            emailContent = emailContent.Replace("_Due_date", resData.balance_due_date.ToString().Split(' ')[0]);
            emailContent = emailContent.Replace("_hotel_name", resData.hotel_name);
            emailContent = emailContent.Replace("_total_rooms", resData.total_rooms.ToString());
            emailContent = emailContent.Replace("_nights", resData.total_nights.ToString());

            string reservationDetails = String.Empty;
            int counter = 1;
            foreach (var client in resDetailData)
            {
                reservationDetails += @"<tr>
                                        <td style = ""padding-bottom:5px;padding-left:25px;padding-right:25px;padding-top:5px;"">
                                             <div style = ""font-family: sans-serif"" >
                                                  <div style = ""font-size: 12px; mso-line-height-alt: 18px; color: #ffffff; line-height: 1.5; font-family: Montserrat, Trebuchet MS, Lucida Grande, Lucida Sans Unicode, Lucida Sans, Tahoma, sans-serif;"">
                                                       <p style = ""margin: 0; mso-line-height-alt: 24px;""><strong><span style = ""font-size:16px;""> RM#" + counter.ToString() + @":&nbsp;</span></strong><span style = ""font-size:16px;""> " + client.client_first_name + " " + client.client_last_name + " - In: " + client.reservation_from.ToString().Split(' ')[0] + " - Out: " + client.reservation_to.ToString().Split(' ')[0] + " - " + roomType(client.room_type) + @" </span></p>
                                                    </div>
                                                </div>
                                          </td>
                                     </tr>";
                counter++;
            }

            emailContent = emailContent.Replace("_reservation_details", reservationDetails);
            emailContent = emailContent.Replace("_download_link", ReservationService.GetBaseUrl() + "Booking/BalancePDF?reservation_id=" + id.ToString());

            ReservationService.sendMail(mailServer.outgoing_mail, resData.reservations_officer_email, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);
            if (resData.reservations_officer_email_2 != null)
                ReservationService.sendMail(mailServer.outgoing_mail, resData.reservations_officer_email_2, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);
            if (resData.reservations_officer_email_3 != null)
                ReservationService.sendMail(mailServer.outgoing_mail, resData.reservations_officer_email_3, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);

            ReservationService.sendMail(mailServer.outgoing_mail, mailServer.incoming_mail, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);

            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult creditMail(int id)
        {
            ReservationViewModel resData = (from res in db.Reservations
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
                                                credit = res.credit,
                                                event_hotel_id = event_hotel.id,
                                                hotel_name = hotel.name,
                                                hotel_rate = hotel.rate,
                                                reservations_officer_name = res.reservations_officer_name,
                                                reservations_officer_email = res.reservations_officer_email,
                                                reservations_officer_email_2 = res.reservations_officer_email_2,
                                                reservations_officer_email_3 = res.reservations_officer_email_3,
                                                reservations_officer_phone = res.reservations_officer_phone,
                                                created_by = res.created_by,
                                                opener = res.opener,
                                                closer = res.closer,
                                                opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                                                closer_name = closer.full_name == null ? "No Closer Assigned" : closer.full_name,
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
                                                created_at = res.created_at,
                                                reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == res.id).Select(resCre => new ReservationCreditCardViewModel
                                                {
                                                    id = resCre.id,
                                                    security_code = resCre.security_code,
                                                    credit_card_number = resCre.credit_card_number,
                                                    card_expiration_date = resCre.card_expiration_date
                                                }).ToList(),
                                                hotel_name_special = res.hotel_name,
                                                transactions = db.Transactions.Where(t => t.reservation_id == res.id).Select(tr => new TransactionViewModel
                                                {
                                                    id = tr.id,
                                                    reservation_id = tr.reservation_id,
                                                    amount = tr.amount,
                                                    transaction_id = tr.transaction_id
                                                }).ToList(),
                                                cancelation_fees = res.cancelation_fees
                                                //profit = calculateProfit(res.id).profit
                                            }).Where(r => r.id == id).FirstOrDefault();
            MailServer mailServer = db.MailServers.Where(ms => ms.type == 5).FirstOrDefault();

            resData.welcome_message = mailServer.welcome_message.Replace("_Name", resData.reservations_officer_name);

            string confirmationMailPath = Path.Combine(Server.MapPath("~/MailTemplates/credit.html"));
            StreamReader streamReader = new StreamReader(confirmationMailPath);
            string emailContent = streamReader.ReadToEnd();

            emailContent = emailContent.Replace("_Name", resData.reservations_officer_name);
            emailContent = emailContent.Replace("_hotel_name", resData.hotel_name);
            emailContent = emailContent.Replace("_credit_date", DateTime.Now.ToString().Split(' ')[0]);
            emailContent = emailContent.Replace("_credit", resData.credit.ToString());
            emailContent = emailContent.Replace("_download_link", ReservationService.GetBaseUrl() + "/Booking/CreditPDF?reservation_id=" + id.ToString());

            ReservationService.sendMail(mailServer.outgoing_mail, resData.reservations_officer_email, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);
            if (resData.reservations_officer_email_2 != null)
                ReservationService.sendMail(mailServer.outgoing_mail, resData.reservations_officer_email_2, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);
            if (resData.reservations_officer_email_3 != null)
                ReservationService.sendMail(mailServer.outgoing_mail, resData.reservations_officer_email_3, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);
            ReservationService.sendMail(mailServer.outgoing_mail, mailServer.incoming_mail, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);

            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult itiniraryMail(int id)
        {
            ReservationService.itiniraryMail(id);


            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }

        public string roomType(int? type)
        {
            if (type == 1)
                return "Single";
            else if (type == 2)
                return "Double";
            else if (type == 3)
                return "Triple";
            else
                return "Quad";
        }

        public JsonResult copyReservation(int id)
        {
            var resData = (from res in db.Reservations
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
                               credit = res.credit,
                               refund = res.refund,
                               company_id = res.company_id,
                               event_hotel_id = res.event_hotel_id,
                               hotel_name = res.hotel_name,
                               reservations_officer_name = res.reservations_officer_name,
                               reservations_officer_email = res.reservations_officer_email,
                               reservations_officer_phone = res.reservations_officer_phone,
                               opener = res.opener,
                               closer = res.closer,
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
                               reservation_avg_price_before_tax = res.reservation_avg_price_before_tax,
                               reservation_avg_price = res.reservation_avg_price,
                               vendor_avg_price = res.vendor_avg_price,
                               paid_amount = res.paid_amount,
                               total_amount_after_tax = res.total_amount_after_tax,
                               total_amount_from_vendor = res.total_amount_from_vendor,
                               advance_reservation_percentage = res.advance_reservation_percentage,
                               tax_amount = res.tax_amount,
                               shift_name = ((Shift)res.shift).ToString(),
                               card_expiration_date = res.card_expiration_date,
                               credit_card_number = res.credit_card_number,
                               security_code = res.security_code,
                           }).Where(r => r.id == id).FirstOrDefault();

            Reservation copiedReservaton = new Reservation();
            copiedReservaton.paid_amount = 0;
            copiedReservaton.total_amount = 0;
            copiedReservaton.total_amount_after_tax = 0;
            copiedReservaton.total_amount_from_vendor = 0;
            copiedReservaton.currency = resData.currency;
            copiedReservaton.event_hotel_id = resData.event_hotel_id;
            copiedReservaton.hotel_name = resData.hotel_name;
            copiedReservaton.tax = resData.tax;
            copiedReservaton.single_price = resData.single_price;
            copiedReservaton.double_price = resData.double_price;
            copiedReservaton.triple_price = resData.triple_price;
            copiedReservaton.quad_price = resData.quad_price;
            copiedReservaton.vendor_single_price = resData.vendor_single_price;
            copiedReservaton.vendor_douple_price = resData.vendor_douple_price;
            copiedReservaton.vendor_triple_price = resData.vendor_triple_price;
            copiedReservaton.vendor_quad_price = resData.vendor_quad_price;
            copiedReservaton.total_nights = 0;
            copiedReservaton.total_rooms = 0;
            copiedReservaton.tax_amount = 0;
            copiedReservaton.active = 1;
            copiedReservaton.created_by = Session["id"].ToString().ToInt();
            copiedReservaton.created_at = DateTime.Now;

            db.Reservations.Add(copiedReservaton);
            db.SaveChanges();

            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult saveEmptyRoom(ReservationDetailViewModel detailViewModel)
        {
            if(detailViewModel.id == 0)
            { 
                ReservationDetail emptyRoom = AutoMapper.Mapper.Map<ReservationDetailViewModel, ReservationDetail>(detailViewModel);
                emptyRoom.payment_to_vendor_deadline = DateTime.Now;
                db.ReservationDetails.Add(emptyRoom);
                db.SaveChanges();
            }
            else
            {
                ReservationDetail oldEmptyRoom = db.ReservationDetails.Find(detailViewModel.id);
                oldEmptyRoom.additional_hotel_name = detailViewModel.additional_hotel_name;
                oldEmptyRoom.additional_show_name = detailViewModel.additional_show_name;
                oldEmptyRoom.room_type = detailViewModel.room_type;
                oldEmptyRoom.vendor_cost = detailViewModel.vendor_cost;
                oldEmptyRoom.vendor_code = detailViewModel.vendor_code;
                oldEmptyRoom.reservation_from = detailViewModel.reservation_from;
                oldEmptyRoom.reservation_to = detailViewModel.reservation_to;
                //oldEmptyRoom.payment_to_vendor_deadline = detailViewModel.payment_to_vendor_deadline;

                db.SaveChanges();
            }
            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }
    }

}