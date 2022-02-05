﻿using Agency.Helpers;
using Agency.Models;
using Agency.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 
namespace Agency.Services
{
    public class ReservationService
    {
        public static AgencyDbContext db = new AgencyDbContext();
        public static ReservationViewModel calculateTotalandVendor(int res_id)
        {
            double? total_amount = 0;
            double? tax_amount = 0;
            double? total_amount_after_tax = 0;
            double? total_amount_from_vendor = 0;
            int? total_nights = 0;
            Reservation reservation = db.Reservations.Find(res_id);
            List<ReservationDetail> reservationDetails = db.ReservationDetails.Where(rd => rd.reservation_id == res_id).ToList();
            //Event event_obj = db.Events.Find(even_id);
            EventHotel eventHotel = db.EventHotels.Find(reservation.event_hotel_id);
            foreach (var resDetail in reservationDetails)
            {
                double? room_price = 0;
                double? vendor_room_price = resDetail.vendor_cost;
                if (resDetail.room_type == 1)
                {
                    room_price = reservation.single_price;
                }
                else if (resDetail.room_type == 2)
                {
                    room_price = reservation.double_price;
                }
                else if (resDetail.room_type == 3)
                {
                    room_price = reservation.triple_price;
                }
                else if (resDetail.room_type == 4)
                {
                    room_price = reservation.quad_price;
                }
                else
                {
                    room_price = reservation.single_price;
                }

                total_amount += resDetail.amount ;
                total_amount_after_tax += resDetail.amount_after_tax;
                total_amount_from_vendor += (vendor_room_price*(resDetail.no_of_days-1));
                tax_amount += resDetail.tax;
                total_nights += resDetail.no_of_days-1;
            }
            ReservationViewModel reservationViewModel = new ReservationViewModel();
            reservationViewModel.total_amount = total_amount;
            reservationViewModel.total_amount_after_tax = total_amount_after_tax;
            reservationViewModel.total_amount_from_vendor = total_amount_from_vendor;
            reservationViewModel.tax_amount = tax_amount;
            reservationViewModel.total_nights = total_nights;
            reservationViewModel.total_rooms = reservationDetails.Count();
            return reservationViewModel;
        }

        public static void UpdateTotals(int reservation_id,bool fromWebSite=false)
        {
            Reservation reservation = db.Reservations.Find(reservation_id);

            reservation.total_rooms = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Count();
            reservation.total_amount = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.amount).Sum();
            reservation.total_amount_after_tax = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.amount_after_tax).Sum();
            reservation.tax_amount = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.tax).Sum();
            reservation.total_nights = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.no_of_days - 1).Sum();
            reservation.reservation_avg_price_before_tax = db.ReservationDetails.Where(resDet => resDet.reservation_id == reservation.id && reservation.is_canceled != 1).Select(resDet => resDet.amount).Sum() / reservation.total_nights;
            reservation.reservation_avg_price = db.ReservationDetails.Where(resDet => resDet.reservation_id == reservation.id && reservation.is_canceled != 1).Select(resDet => resDet.amount_after_tax).Sum() / reservation.total_nights;
            reservation.vendor_avg_price = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.vendor_cost).Sum() / reservation.total_nights;
            reservation.total_amount_from_vendor = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.vendor_cost).Sum();

            reservation.updated_at = DateTime.Now;
            if(fromWebSite)
                reservation.updated_by = HttpContext.Current.Session["id"].ToString().ToInt();
            db.SaveChanges();
        }

        public static StatisticsViewModel calculateStatistics()
        {
            StatisticsViewModel statisticsViewModel = new StatisticsViewModel();
            var reservation = db.Reservations;
            if (reservation != null)
            {
                statisticsViewModel.total_amount_after_tax_sum = db.Reservations.Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum();
                statisticsViewModel.collected = db.Reservations.Where(t => t.is_canceled != 1).Select(t => t.paid_amount).Sum();
                statisticsViewModel.collected_percentage = (statisticsViewModel.collected / statisticsViewModel.total_amount_after_tax_sum)*100;

                statisticsViewModel.balance = db.Reservations.Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum() - statisticsViewModel.collected;
                statisticsViewModel.balance_percentage = (statisticsViewModel.balance / statisticsViewModel.total_amount_after_tax_sum)*100;

                double? reservation_cancelation_fees = 0.0;
                List<Reservation> reservations = db.Reservations.Where(c => c.is_canceled == 1 && c.cancelation_fees != null).ToList();

                if (reservations.Count() > 0)
                {
                    reservation_cancelation_fees = db.Reservations.Where(c => c.is_canceled == 1 && c.cancelation_fees != null).Select(c => c.cancelation_fees).Sum();
                }

                statisticsViewModel.profit = db.Reservations.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum() - db.Reservations.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Where(t => t.is_canceled != 1).Select(t => t.total_amount_from_vendor).Sum();
                statisticsViewModel.profit += reservation_cancelation_fees;

                statisticsViewModel.total_amount_after_tax_sum_for_profit = db.Reservations.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum();
                statisticsViewModel.profit_percentage = (statisticsViewModel.profit / statisticsViewModel.total_amount_after_tax_sum_for_profit) *100;

                statisticsViewModel.refund = db.Reservations.Where(t => t.is_canceled != 1).Select(t => t.refund).Sum();
                statisticsViewModel.refund_percentage = (statisticsViewModel.refund / statisticsViewModel.total_amount_after_tax_sum) * 100;
                
                statisticsViewModel.total_nights = db.Reservations.Where(t => t.is_canceled != 1).Select(n => n.total_nights).Sum();

            }
            return statisticsViewModel;
        }
    }
}