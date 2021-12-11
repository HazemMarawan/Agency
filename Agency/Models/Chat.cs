using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class Chat
    {
        [Key]
        public int id { get; set; }
        public int? from_user { get; set; }
        public int? to_user { get; set; }
        public string message { get; set; }
        public DateTime? created_at { get; set; }
    }
}