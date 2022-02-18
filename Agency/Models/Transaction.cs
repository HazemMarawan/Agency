using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class Transaction
    {
        public int id { get; set; }
        public double? amount { get; set; }
        public string transaction_id { get; set; }

        [ForeignKey("Reservation")]
        public int? reservation_id { get; set; }
        public Reservation Reservation { get; set; }

        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}