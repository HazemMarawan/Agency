using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class HotelLocation
    {
        [Key]
        public int id { get; set; }
        public string distance { get; set; }
        public string map_link { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        [ForeignKey("Location")]
        public int? location_id { get; set; }
        public Location Location { get; set; }
        [ForeignKey("Hotel")]
        public int? hotel_id { get; set; }
        public Hotel Hotel { get; set; }
    }
}