using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class ReservationLogViewModel
    {
        public int id { get; set; }
        public int? reservation_id { get; set; }
        public int? user_id { get; set; }
        public string full_name { get; set; }
        public string action { get; set; }
        public string description { get; set; }
        public DateTime? created_at { get; set; }
        public string string_created_at { get; set; }
    }
}