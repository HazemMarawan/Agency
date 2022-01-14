using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class EventViewModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string secret_key { get; set; }
        public string description { get; set; }
        public DateTime event_from { get; set; }    
        public DateTime to { get; set; }
        public DateTime due_date { get; set; }
        public List<int> event_hotels_ids { get; set; }
        public double? tax { get; set; }
        public double? advance_reservation_percentage { get; set; }
        public int? is_special { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public List<EventHotelViewModel> hotels { get; set; }
        public int? location_id { get; set; }
        public string location_name { get; set; }

    }
}