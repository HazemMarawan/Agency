using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class DashboardViewModel
    {
        public List<HotelViewModel> topSellingHotels { get; set; }
        public List<ReservationLogViewModel> logs { get; set; }
        public UserViewModel userData { get; set; }
        public List<ChatViewModel> LatestChats { get; set; }
        public List<ReservationViewModel> CheckinCompanies { get; set; }
        public List<TaskViewModel> currentUserTasks { get; set; }
        public List<ReservationViewModel> balanceDueDateReservations { get; set; }
        public int TotalNotes { get; set; }
    }
}