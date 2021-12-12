using Agency.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class ReservationViewModel
    {
        [Key]
        public int id { get; set; }
        public string reservations_officer_name { get; set; }
        public string reservations_officer_phone { get; set; }
        public string reservations_officer_email { get; set; }
        public double? total_amount { get; set; }
        public double? total_amount_after_tax { get; set; }
        public double? total_amount_from_vendor { get; set; }
        public double? tax { get; set; }
        public double? tax_amount { get; set; }
        public double? advance_reservation_percentage { get; set; }
        public double? financial_advance { get; set; }
        public double? financial_due { get; set; }
        public DateTime? financial_advance_date { get; set; }
        public DateTime? financial_due_date { get; set; }
        public int? status { get; set; } //paid or partially
        public double? single_price { get; set; }
        public double? double_price { get; set; }
        public double? triple_price { get; set; }
        public double? quad_price { get; set; }
        public int? vendor_id { get; set; }
        public int? currency { get; set; }
        public string string_currency { get; set; }
        public double? profit { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public int? company_id { get; set; }
        public Company Company { get; set; }
        public int? event_hotel_id { get; set; }
        public double? hotel_rate { get; set; }
        public string company_name { get; set; }
        public string created_at_string { get; set; }
        public string updated_at_string { get; set; }
        public string financial_due_date_string { get; set; }
        public string hotel_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public double? paid_amount { get; set; }
        public int? opener { get; set; }
        public int? closer { get; set; }
        public double? vendor_single_price { get; set; }
        public double? vendor_douple_price { get; set; }
        public double? vendor_triple_price { get; set; }
        public double? vendor_quad_price { get; set; }
        public int? total_rooms { get; set; }
        public int? total_nights { get; set; }
        public DateTime? check_in { get; set; }
        public DateTime? check_out { get; set; }
        public DateTime balance_due_date { get; set; }
        public string string_balance_due_date { get; set; }
        public string string_check_in { get; set; }
        public string string_check_out { get; set; }
        public EventHotel EventHotel { get; set; }
        public string opener_name { get; set; }
        public string closer_name { get; set; }
        public int? shift { get; set; }
        public int? is_canceled { get; set; }
        public string shift_name { get; set; }
        public string event_name { get; set; }
        public double? event_tax { get; set; }
        public double? reservation_avg_price { get; set; }
        public double? vendor_avg_price { get; set; }
        public double? event_single_price { get; set; }
        public double? event_double_price { get; set; }
        public double? event_triple_price { get; set; }
        public double? event_quad_price { get; set; }
        public double? event_vendor_single_price { get; set; }
        public double? event_vendor_double_price { get; set; }
        public double? event_vendor_triple_price { get; set; }
        public double? event_vendor_quad_price { get; set; }
        public string vendor_code { get; set; }
        public double? total_price { get; set; }
        public int hotel_id { get; set; }
        public string hotel_address { get; set; }
        public int single_rooms { get; set; }
        public int double_rooms { get; set; }
        public int triple_rooms { get; set; }
        public int quad_rooms { get; set; }
        public int? single_nights { get; set; }
        public int? double_nights { get; set; }
        public int? triple_nights { get; set; }
        public int? quad_nights { get; set; }
        public List<ReservationDetailViewModel> ReservationDetailVM { get; set; }
        public List<string> hotelFacilities { get; set; }
        public string created_by_name { get; set; }
        public string updated_by_name { get; set; }
        public string city_name { get; set; }
        public string location_name { get; set; }
        public double? vendor_cost { get; set; }


    }
}