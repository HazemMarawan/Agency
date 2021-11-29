using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class ReservationCommentViewModel
    {
        public int id { get; set; }
        public string comment { get; set; }
        public int? parent_id { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public int? reservation_id { get; set; }
        public string created_by_string { get; set; }
        public string created_at_string { get; set; }

    }
}