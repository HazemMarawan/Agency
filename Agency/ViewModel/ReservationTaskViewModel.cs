using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class ReservationTaskViewModel
    {
        public int id { get; set; }
        public string task_title { get; set; }
        public string task_detail { get; set; }
        public int status { get; set; }
        public int task_to_user { get; set; }
        public DateTime? notification_date { get; set; }
        public DateTime? due_date { get; set; }
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
        public string task_to_user_string { get; set; }
    }
}