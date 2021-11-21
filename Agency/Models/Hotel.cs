using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public class Hotel
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public double? rate { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        [ForeignKey("City")]
        public int? city_id { get; set; }
        public City City { get; set; }
        public virtual ICollection<HotelFacilitie> HotelFacilities { get; set; }
        public virtual ICollection<EventHotel> EventHotels { get; set; }
        public virtual ICollection<HotelLocation> HotelLocations { get; set; }
        public virtual ICollection<HotelImage> HotelImages { get; set; }

    }
}