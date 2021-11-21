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
                    room_price = reservation.single_price;
                    vendor_room_price = reservation.vendor_single_price;// > 0 ? reservation.vendor_single_price : eventhotel.vendor_single_price;
                }
                else if (resDetail.room_type == 2)
                {
                    room_price = reservation.double_price;
                    vendor_room_price = reservation.vendor_douple_price; //> 0 ? reservation.vendor_douple_price : eventhotel.vendor_douple_price;

                }
                else if (resDetail.room_type == 3)
                {
                    room_price = reservation.triple_price;
                    vendor_room_price = reservation.vendor_triple_price;// > 0 ? reservation.vendor_triple_price : eventhotel.vendor_triple_price;

                }
                else if (resDetail.room_type == 4)
                {
                    room_price = reservation.quad_price;
                    vendor_room_price = reservation.vendor_quad_price;// > 0 ? reservation.vendor_quad_price : eventhotel.vendor_quad_price;
                }
                total_amount += resDetail.amount ;
                total_amount_after_tax += resDetail.amount_after_tax;
                total_amount_from_vendor += (vendor_room_price*resDetail.no_of_days);
                tax_amount += resDetail.tax;
            }
            ReservationViewModel reservationViewModel = new ReservationViewModel();
            reservationViewModel.total_amount = total_amount;
            reservationViewModel.total_amount_after_tax = total_amount_after_tax;
            reservationViewModel.total_amount_from_vendor = total_amount_from_vendor;
            reservationViewModel.tax_amount = tax_amount;
            return reservationViewModel;
        }
    }
}