using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class ReservationCreditCardViewModel
    {
        public int id { get; set; }
        public string credit_card_number { get; set; }
        public string security_code { get; set; }
        public string card_expiration_date { get; set; }
        public int? reservation_id { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
    }
}