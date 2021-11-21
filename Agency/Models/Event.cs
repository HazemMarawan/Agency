using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class Event
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime event_from { get; set; }
        public DateTime to { get; set; }
        public DateTime due_date { get; set; }
        public double? percentage { get; set; }
        public double? promocode { get; set; }
        public double? tax { get; set; }
        public double? advance_reservation_percentage { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        [ForeignKey("Location")]
        public int? location_id { get; set; }
        public Location Location { get; set; }
        public virtual ICollection<EventHotel> EventHotels { get; set; }
    }
}