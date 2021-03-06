using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Agency.ViewModel;
using Agency.Models;
using Agency.Enums;

namespace Agency.Helpers
{
    public class Notification
    {
        public static AgencyDbContext db = new AgencyDbContext();
        public static List<ReservationViewModel> balanceDueDateDashboardNotifications()
        {
            List<ReservationViewModel> balanceDueDateReservations = (from res in db.Reservations
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
                                                                         //}).Where(r => r.balance_due_date.Year == DateTime.Now.Year && r.balance_due_date.Month == DateTime.Now.Month && r.balance_due_date.Day == DateTime.Now.Day && r.is_canceled == null && r.countPaidToVendorRooms == 0).Take(2).ToList();
                                                                     }).Where(r => r.balance_due_date <= DateTime.Now && r.is_canceled == null && r.total_amount_after_tax != 0 && r.paid_amount < r.total_amount_after_tax).ToList();
            return balanceDueDateReservations;
        }
        public static List<ReservationViewModel> balanceDueDateNotifications()
        {
            List<ReservationViewModel> balanceDueDateReservations = (from res in db.Reservations
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
                                                                     }).Where(r => r.balance_due_date <= DateTime.Now && r.is_canceled == null && r.total_amount_after_tax != 0 && r.paid_amount < r.total_amount_after_tax).ToList();
            return balanceDueDateReservations;
        }
        public static List<ReservationDetailViewModel> payToVendorNotification()
        {
            List<ReservationDetailViewModel> payToVendorReservations = (from resDetail in db.ReservationDetails
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

                                                                        }).Where(s=>s.paid_to_vendor != 1 && s.is_reservation_canceled != 1 && (s.payment_to_vendor_deadline <= DateTime.Now || s.payment_to_vendor_notification_date <= DateTime.Now)).ToList();
            return payToVendorReservations;
        }
    }
}