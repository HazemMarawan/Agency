using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class MailServer
    {
        [Key]
        public int id { get; set; }
        public string outgoing_mail { get; set; }
        public string outgoing_mail_password { get; set; }
        public string outgoing_mail_server { get; set; }
        public string port { get; set; }
        public string title { get; set; }
        public string welcome_message { get; set; }
        public string incoming_mail { get; set; }
        public int? type { get; set; }
        public int? updated_by { get; set; }
        public DateTime? updated_at { get; set; }


    }
}