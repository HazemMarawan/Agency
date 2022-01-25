using Agency.Auth;
using Agency.Helpers;
using Agency.Models;
using Agency.Services;
using Agency.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agency.Controllers
{
    [CustomAuthenticationFilter]
    public class DashboardController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();
        // GET: Dashboard
        public ActionResult Index()
        {
            User currentUser = Session["user"] as User;
            UserViewModel userData = (from user in db.Users
                                      select new UserViewModel
                                      {
                                          id = user.id,
                                          user_name = user.user_name,
                                          full_name = user.full_name,
                                          password = user.password,
                                          type = user.type,
                                          phone1 = user.phone1,
                                          phone2 = user.phone2,
                                          imagePath = user.image,
                                          address1 = user.address1,
                                          address2 = user.address2,
                                          birthDate = user.birthDate,
                                          code = user.code,
                                          email = user.email,
                                          gender = user.gender,
                                          active = user.active,
                                      }).Where(u => u.id == currentUser.id).FirstOrDefault();

            var topSelling = (from hotel in db.Hotels
                              join eventHotel in db.EventHotels on hotel.id equals eventHotel.hotel_id
                              join res in db.Reservations on eventHotel.id equals res.event_hotel_id
                              join resDet in db.ReservationDetails on res.id equals resDet.reservation_id
                              select new ReservationViewModel
                              {
                                  hotel_id = hotel.id,
                                  hotel_name = hotel.name,
                                  total_nights = res.total_nights
                              }).GroupBy(g => g.hotel_id).ToList();

            string cs = ConfigurationManager.ConnectionStrings["DBContextADO"].ConnectionString;

            SqlConnection sql = new SqlConnection(cs);
            sql.Open();

            //Top Selling Hotels by Nights
            SqlCommand comm = new SqlCommand(@"select top 10 sum(total_nights) as total_nights, Hotels.name as hotel_name, Cities.name as city_name, Hotels.rate,(select top 1 HotelImages.path from HotelImages where HotelImages.hotel_id = Hotels.id) as hotel_image
											from Reservations 
                                            join EventHotels on Reservations.event_hotel_id = EventHotels.id
                                            join Hotels on EventHotels.hotel_id = Hotels.id
                                            join Cities on Cities.id = Hotels.city_id
                                            group by Hotels.id, Hotels.name, Cities.name, Hotels.rate", sql);
            SqlDataReader reader = comm.ExecuteReader();
            List<HotelViewModel> hotels  = new List<HotelViewModel>();

            while (reader.Read())
            {
                hotels.Add(new HotelViewModel { total_nights = !String.IsNullOrEmpty(reader["total_nights"].ToString())? reader["total_nights"].ToString().ToInt():0, 
                    name = reader["hotel_name"].ToString(),
                    cityName = reader["city_name"].ToString(),
                    rating = Convert.ToDouble(reader["rate"].ToString()),
                    path = reader["hotel_image"].ToString(),
                });
            }
            reader.Close();

            //Latest Messages
            comm = new SqlCommand(@"select top 5 orginalChats.from_user,users.full_name,users.image, (select top 1 message from Chats where from_user = orginalChats.from_user order by created_at desc) as latest_message
                                    from chats as orginalChats
                                    inner join users on orginalChats.from_user = users.id
                                    where orginalChats.to_user = 1
                                    group by orginalChats.from_user,users.full_name,users.image", sql);
            reader = comm.ExecuteReader();
            List<ChatViewModel> LatestChats = new List<ChatViewModel>();

            while (reader.Read())
            {
                LatestChats.Add(new ChatViewModel
                {
                    strings_from_user = reader["full_name"].ToString(),
                    message = reader["latest_message"].ToString(),
                    image = reader["image"].ToString()
                });
            }
            reader.Close();
            List<ReservationLogViewModel> reservationLog = (from resLog in db.ReservationLogs
                                                   join user in db.Users on resLog.user_id equals user.id
                                                   select new ReservationLogViewModel
                                                   {
                                                        action = resLog.action,
                                                        description = resLog.description,
                                                        full_name = user.full_name,
                                                        created_at = resLog.created_at
                                                   }).Take(10).ToList();
            DateTime nextWeekDate = DateTime.Now.AddDays(7);
            List<ReservationViewModel> CheckinCompanies = (
                              
                                 from res in db.Reservations
                                 join company in db.Companies on res.company_id equals company.id
                                 join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                                 join even in db.Events on event_hotel.event_id equals even.id
                                 join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                                 select new ReservationViewModel
                                 {
                                        id = res.id,
                                        hotel_name = hotel.name,
                                        total_rooms = res.total_rooms,
                                        total_nights = res.total_nights,
                                        company_name = company.name,
                                        check_in = res.check_in,
                                        is_canceled = res.is_canceled

                                 }).Where(r=>r.is_canceled != 1 && ((DateTime)r.check_in).Year == nextWeekDate.Year && ((DateTime)r.check_in).Month == nextWeekDate.Month && ((DateTime)r.check_in).Day == nextWeekDate.Day).Take(10).ToList();

            List<TaskViewModel> currentUserTasks = (from task in db.Tasks
                                 join user in db.Users on task.created_by equals user.id
                                 join user_task in db.UserTasks on task.id equals user_task.task_id
                                 select new TaskViewModel
                                 {
                                     id = task.id,
                                     user_task_id = user_task.id,
                                     title = task.title,
                                     description = task.description,
                                     stringCreatedAt = task.created_at.ToString(),
                                     user_id = user_task.user_id,
                                     status = user_task.status,
                                     created_at = task.created_at,
                                     created_by = task.created_by,
                                     stringCreatedToBy = user.full_name

                                 }).Where(s => s.user_id == currentUser.id).OrderByDescending(s => s.created_at).Take(2).ToList();

           
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            dashboardViewModel.topSellingHotels = hotels;
            dashboardViewModel.logs = reservationLog;
            dashboardViewModel.userData = userData;
            dashboardViewModel.LatestChats = LatestChats;
            dashboardViewModel.CheckinCompanies = CheckinCompanies;
            dashboardViewModel.currentUserTasks = currentUserTasks;
            dashboardViewModel.balanceDueDateReservations = Notification.balanceDueDateNotifications();
            dashboardViewModel.TotalNotes = db.Notes.Where(n => n.created_by == currentUser.id).ToList().Count();
            return View(dashboardViewModel);
        }

        public JsonResult revenueReservations()
        {
            string cs = ConfigurationManager.ConnectionStrings["DBContextADO"].ConnectionString;

            SqlConnection sql = new SqlConnection(cs);
            sql.Open();

            SqlCommand comm = new SqlCommand(@"select SUM(mainReservation.paid_amount) as collected,SUM(mainReservation.total_amount_after_tax-mainReservation.paid_amount) as balance ,
                                            (select SUM(pr.total_amount_after_tax -pr.total_amount_from_vendor)  from Reservations pr where pr.paid_amount = pr.total_amount_after_tax and YEAR(pr.created_at) = YEAR(mainReservation.created_at) and Month(pr.created_at) = Month(mainReservation.created_at))  as profit
                                            ,CONCAT( YEAR(created_at),'-' ,MONTH(created_at)) as date
                                            from Reservations mainReservation where mainReservation.is_canceled is null
                                            group by YEAR(mainReservation.created_at),MONTH(mainReservation.created_at)
                                            ", sql);
            SqlDataReader reader = comm.ExecuteReader();
            List<double> yAxisCollected = new List<double>();
            List<double> yAxisBalance = new List<double>();
            List<double> yAxisProfit = new List<double>();
            List<string> xAxis = new List<string>();

            while (reader.Read())
            {
                yAxisCollected.Add(Convert.ToDouble(reader["collected"].ToString()));
                yAxisBalance.Add(Convert.ToDouble(reader["balance"].ToString()));
                yAxisProfit.Add(Convert.ToDouble(reader["profit"].ToString()));
                xAxis.Add(reader["date"].ToString());
            }
            reader.Close();

            StatisticsViewModel statisticsViewModel = ReservationService.calculateStatistics();

            double? profit = statisticsViewModel.profit;
            double? refund = statisticsViewModel.refund;

            double? collected = statisticsViewModel.collected;
            double? balance = statisticsViewModel.balance;


            double? profitPercentage = statisticsViewModel.profit_percentage;
            double? refundPercentage = statisticsViewModel.refund_percentage; ;


            double? collectedPercentage = statisticsViewModel.collected_percentage ;
            double? balancePercentage = statisticsViewModel.balance_percentage ;

            return Json(new { 
                xAxis = xAxis,
                yAxisCollected = yAxisCollected,
                yAxisBalance = yAxisBalance,
                yAxisProfit = yAxisProfit,
                profit = profit,
                refund = refund,
                collected = collected,
                balance = balance,
                profitPercentage = profitPercentage,
                refundPercentage = refundPercentage,
                collectedPercentage = collectedPercentage,
                balancePercentage = balancePercentage
           }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult dailyReservations()
        {
            string cs = ConfigurationManager.ConnectionStrings["DBContextADO"].ConnectionString;

            SqlConnection sql = new SqlConnection(cs);
            sql.Open();

            SqlCommand comm = new SqlCommand(@"select SUM(paid_amount) as collected,SUM(total_amount_after_tax-paid_amount) as balance , SUM(total_amount_after_tax - total_amount_from_vendor) as profit,CONCAT( YEAR(created_at),'-' ,MONTH(created_at),'-' ,DAY(created_at)) as date from Reservations where created_at >= DATEADD(DAY,-7,GETDATE())  group by YEAR(created_at),MONTH(created_at),DAY(created_at)", sql);
            SqlDataReader reader = comm.ExecuteReader();
            List<int> yAxisCollected = new List<int>();
            List<int> yAxisBalance = new List<int>();
            List<int> yAxisProfit = new List<int>();
            List<string> xAxis = new List<string>();

            while (reader.Read())
            {
                yAxisCollected.Add(reader["collected"].ToString().ToInt());
                yAxisBalance.Add(reader["balance"].ToString().ToInt());
                yAxisProfit.Add(reader["profit"].ToString().ToInt());
                xAxis.Add(reader["date"].ToString());
            }
            reader.Close();
            
            return Json(new
            {
                xAxis = xAxis,
                yAxisCollected = yAxisCollected,
                yAxisBalance = yAxisBalance,
                yAxisProfit = yAxisProfit
            }, JsonRequestBehavior.AllowGet);
        }


    }
}