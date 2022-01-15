using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class ReservationCreditCard
    {
        [Key]
        public int id { get; set; }
        public string credit_card_number { get; set; }
        public string security_code { get; set; }
        public string card_expiration_date { get; set; }
        [ForeignKey("Reservation")]
        public int? reservation_id { get; set; }
        public Reservation Reservation { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}