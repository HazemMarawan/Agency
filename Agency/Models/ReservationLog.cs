using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class ReservationLog
    {
        [Key]
        public int id { get; set; }
        public int? user_id { get; set; }
        public int? reservation_id { get; set; }
        public string action { get; set; }
        public string description { get; set; }
        public DateTime? created_at { get; set; }
    }
}