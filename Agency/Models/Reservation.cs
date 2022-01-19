using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class Reservation
    {
        [Key]
        public int id { get; set; }
        public string reservations_officer_name { get; set; }
        public string reservations_officer_phone { get; set; }
        public string reservations_officer_email { get; set; }
        public int payment_type { get; set; }
        public string credit_card_number { get; set; }
        public string security_code { get; set; }
        public string card_expiration_date { get; set; }
        public double? total_amount { get; set; }
        public double? total_amount_after_tax { get; set; }
        public double? paid_amount { get; set; }
        public double? total_amount_from_vendor { get; set; }
        public double? advance_reservation_percentage { get; set; }
        public double? tax { get; set; }
        public double? tax_amount { get; set; }
        public double? financial_advance { get; set; }
        public double? financial_due { get; set; }
        public int? status { get; set; } //paid or partially
        public double? single_price { get; set; }
        public double? double_price { get; set; }
        public double? triple_price { get; set; }
        public double? quad_price { get; set; }
        public int? currency { get; set; }
        public int? opener { get; set; }
        public int? closer { get; set; }
        public double? vendor_single_price { get; set; }
        public double? vendor_douple_price { get; set; }
        public double? vendor_triple_price { get; set; }
        public double? vendor_quad_price { get; set; }
        public double? reservation_avg_price_before_tax { get; set; }
        public double? reservation_avg_price { get; set; }
        public double? vendor_avg_price { get; set; }
        public double? total_price { get; set; }
        public double? credit { get; set; }
        public double? refud { get; set; }
        public int? total_rooms { get; set; }
        public int? total_nights { get; set; }
        public DateTime? check_in { get; set; }
        public DateTime? check_out { get; set; }
        public double? profit { get; set; }
        public int? shift { get; set; }
        public int? is_canceled { get; set; }
        [DefaultValue(0)]
        public int is_refund { get; set; }
        public int? active { get; set; }
        public DateTime? financial_advance_date { get; set; }
        public DateTime? financial_due_date { get; set; }
        public DateTime? balance_due_date { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        [ForeignKey("Company")]
        public int? company_id { get; set; }
        public Company Company { get; set; }

        [ForeignKey("Vendor")]
        public int? vendor_id { get; set; }
        public Vendor Vendor { get; set; }

        [ForeignKey("EventHotel")]
        public int? event_hotel_id { get; set; }
        public EventHotel EventHotel { get; set; }
        public virtual ICollection<ReservationDetail> ReservationDetail { get; set; }
        public virtual ICollection<ReservationComment> ReservationComments { get; set; }
        public virtual ICollection<ReservationTask> ReservationTasks { get; set; }
        public virtual ICollection<ReservationCreditCard> ReservationCreditCards { get; set; }

    }
}