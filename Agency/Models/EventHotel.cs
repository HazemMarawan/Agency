using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class EventHotel
    {
        [Key]
        public int id { get; set; }
        public double? single_price { get; set; }
        public int? single_available_rooms { get; set; }
        public double? double_price { get; set; }
        public int? double_available_rooms { get; set; }
        public double? triple_price { get; set; }
        public int? triple_available_rooms { get; set; }
        public double? quad_price { get; set; }
        public double? vendor_single_price { get; set; }
        public double? vendor_douple_price { get; set; }
        public double? vendor_triple_price { get; set; }
        public double? vendor_quad_price { get; set; }
        public int? currency { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }

        [ForeignKey("Event")]
        public int? event_id { get; set; }
        public Event Event { get; set; }

        [ForeignKey("Hotel")]
        public int? hotel_id { get; set; }
        public Hotel Hotel { get; set; }

        [ForeignKey("Vendor")]
        public int? vendor_id { get; set; }
        public Vendor Vendor { get; set; }
        public virtual ICollection<EventHotelBenefit> HotelBenefits { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }


    }
}