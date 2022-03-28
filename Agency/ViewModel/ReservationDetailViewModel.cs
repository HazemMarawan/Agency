using Agency.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class ReservationDetailViewModel
    {
        public int id { get; set; }
        public int? parent_id { get; set; }
        public int res_id { get; set; }
        public double? amount { get; set; }
        public double? amount_after_tax { get; set; }
        public double? tax { get; set; }
        public double? room_price { get; set; }
        public int? room_type { get; set; }
        public DateTime reservation_from { get; set; }
        public DateTime reservation_to { get; set; }
        public string string_reservation_from { get; set; }
        public string string_reservation_to { get; set; }
        public string event_name { get; set; }
        public string hotel_name { get; set; }
        public string company_name { get; set; }
        public string string_currency { get; set; }
        public string vendor_code { get; set; }
        public int? is_canceled { get; set; }
        public int? is_reservation_canceled { get; set; }
        public double? price_per_night { get; set; }
        public string cancelation_policy { get; set; }
        public string additional_show_name { get; set; }
        public string additional_hotel_name { get; set; }
        public int? no_of_days { get; set; }
        public int? active { get; set; }
        public int? is_transfered { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public int? client_id { get; set; }
        public Client Client { get; set; }
        public int? reservation_id { get; set; }
        public Reservation Reservation { get; set; }
        public string client_first_name { get; set; }
        public string client_last_name { get; set; }
        public int? currency  { get; set; }
        public double? vendor_cost { get; set; }
        public string string_payment_to_vendor_deadline { get; set; }
        public DateTime? payment_to_vendor_deadline { get; set; }
        public DateTime? paid_to_vendor_date { get; set; }
        public int? notify { get; set; }
        public int? paid_to_vendor { get; set; }
        public double? amount_paid_to_vendor { get; set; }
        public DateTime? payment_to_vendor_notification_date { get; set; }
        public string string_spayment_to_vendor_notification_date { get; set; }
        public int? confirmation_id { get; set; }

        public List<AdditionalReservationViewModel> guestReservations { get; set; }
        public List<DateTime> reservationsFrom { get; set; }
        public List<DateTime> reservationsTo { get; set; }
        public List<string> vendorCodes { get; set; }
        public List<double> vendorCosts { get; set; }
        public List<double> roomPrices { get; set; }
        public List<int> ids { get; set; }
        public List<int> reservationIds { get; set; }
        public List<int> parentIds { get; set; }

    }
}