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
    public class CreditController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: Credit
        public ActionResult Index()
        {
            if(Request.IsAjaxRequest())
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
                                   credit= res.credit,
                                   refund = res.refund,
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
                                   is_refund = res.is_refund,
                                   created_at = res.created_at,
                                   created_at_string = res.created_at.ToString()
                                   //profit = calculateProfit(res.id).profit

                               }).Where(r => r.is_canceled == null && r.credit != null && r.credit != 0);
               
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
            return View();
        }
    }
}