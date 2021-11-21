using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class VendorViewModel
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string NamePlusCode { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string string_created_at { get; set; }
    }
}