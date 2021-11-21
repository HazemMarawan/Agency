using Agency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class HotelLocationViewModel
    {
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
        public int? location_id { get; set; }
        public Location Location { get; set; }
        public int? hotel_id { get; set; }
        public Hotel Hotel { get; set; }
        public List<HotelLocationViewModel> HotelLocations { get; set; }
        public string hotel_name { get; set; }
        public int hotel_city_id { get; set; }
        public string hotel_city_name { get; set; }
        public double? hotel_rate { get; set; }
    }
}