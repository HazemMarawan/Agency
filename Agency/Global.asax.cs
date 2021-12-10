using Agency.Models;
using Agency.ViewModel;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Agency
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Mapper.CreateMap<UserViewModel, User>();
            Mapper.CreateMap<CityViewModel, City>();
            Mapper.CreateMap<HotelViewModel, Hotel>();
            Mapper.CreateMap<EventViewModel, Event>();
            Mapper.CreateMap<EventHotelViewModel, EventHotel>();
            Mapper.CreateMap<ReservationDetailViewModel, ReservationDetail>();
            Mapper.CreateMap<LocationViewModel, Location>(); 
            Mapper.CreateMap<HotelLocationViewModel, HotelLocation>();
            Mapper.CreateMap<ReservationViewModel, Reservation>();
            Mapper.CreateMap<VendorViewModel, Vendor>();
            Mapper.CreateMap<ReservationCommentViewModel, ReservationComment>();
            Mapper.CreateMap<ReservationTaskViewModel, ReservationTask>();
            Mapper.CreateMap<ChatViewModel, Chat>();


        }
    }
}
