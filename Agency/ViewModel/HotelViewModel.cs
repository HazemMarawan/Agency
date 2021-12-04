using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class HotelViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public double? rating { get; set; }
        public string cityName { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public int? city_id { get; set; }
        public int? event_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string facilities_string { get; set; }
        public List<int> facilities { get; set; }
        public List<HotelFacilitieViewModel> hotelFacilities { get; set; }
        public int? location_id { get; set; }
        public List<HotelImageViewModel> HotelImagesVM { get; set; }
        public List<HttpPostedFileBase> images { get; set; }
        public int total_nights { get; set; }
        public string path { get; set; }

    }
}