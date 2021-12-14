using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class EventHotelViewModel
    {
        public int id { get; set; }
        public int event_hotel_id { get; set; }
        public string name { get; set; }
        public double? rate { get; set; }
        public int fixed_rate { get; set; }
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
        public int? location_id { get; set; }
        public string distance { get; set; }
        public int? currency { get; set; }
        public string string_currency { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public int? event_id { get; set; }
        public int? hotel_id { get; set; }
        public int? vendor_id { get; set; }
        public List<int> benefits { get; set; }
        public List<EventHotelBenefitViewModel> eventHotelBenefit { get; set; }
        public List<HotelImageViewModel> hotelImages { get; set; }
    }
}