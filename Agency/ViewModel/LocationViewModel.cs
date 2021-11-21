using Agency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class LocationViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string distance { get; set; }
        public string map_link { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public int? city_id { get; set; }
        public City City { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual List<HotelLocationViewModel> HotelLocations { get; set; }
        public string location_city_name { get; set; }

    }
}