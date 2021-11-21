using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class Vendor
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public virtual ICollection<EventHotel> EventHotels { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }

    }
}