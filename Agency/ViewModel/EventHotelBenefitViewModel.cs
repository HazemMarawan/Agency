using System;

namespace Agency.ViewModel
{
    public class EventHotelBenefitViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public int? hotel_id { get; set; }
        public int? event_hotel_id { get; set; }
        public int? benefit_id { get; set; }
    }
}