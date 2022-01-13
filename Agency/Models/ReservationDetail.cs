﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class ReservationDetail
    {
        [Key]
        public int id { get; set; }
        public double? amount { get; set; }
        public double? amount_after_tax { get; set; }
        public double? tax { get; set; }
        public double? room_price { get; set; }
        public int? room_type { get; set; }
        public DateTime reservation_from { get; set; }
        public DateTime reservation_to { get; set; }
        public int? no_of_days { get; set; }
        public int? confirmation_id { get; set; }
        public string vendor_code { get; set; }
        public string cancelation_policy { get; set; }
        public double? vendor_cost { get; set; }
        public DateTime? payment_to_vendor_deadline { get; set; }
        public DateTime? paid_to_vendor_date { get; set; }
        public int? is_canceled { get; set; }
        public int? notify { get; set; }
        public int? paid_to_vendor { get; set; }
        public double? amount_paid_to_vendor { get; set; }
        public DateTime? payment_to_vendor_notification_date { get; set; }
        public int? active { get; set; }
        public int? is_transfered { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        [ForeignKey("Client")]
        public int? client_id { get; set; }
        public Client Client { get; set; }
        [ForeignKey("Reservation")]
        public int? reservation_id { get; set; }
        public Reservation Reservation { get; set; }
    }
}