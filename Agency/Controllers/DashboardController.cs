using Agency.Auth;
using Agency.Helpers;
using Agency.Models;
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
                hotels.Add(new HotelViewModel { total_nights = reader["total_nights"].ToString().ToInt(), 
                    name = reader["hotel_name"].ToString(),
                    cityName = reader["city_name"].ToString(),
                    rating = Convert.ToDouble(reader["rate"].ToString()),
                    path = reader["hotel_image"].ToString(),
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
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            dashboardViewModel.topSellingHotels = hotels;
            dashboardViewModel.logs = reservationLog;
            return View(dashboardViewModel);
        }

        public JsonResult revenueReservations()
        {
            string cs = ConfigurationManager.ConnectionStrings["DBContextADO"].ConnectionString;

            SqlConnection sql = new SqlConnection(cs);
            sql.Open();

            SqlCommand comm = new SqlCommand(@"select SUM(paid_amount) as collected,SUM(total_amount_after_tax-paid_amount) as balance , SUM(total_amount_after_tax -total_amount_from_vendor) as profit,CONCAT( YEAR(created_at),'-' ,MONTH(created_at)) as date from Reservations group by YEAR(created_at),MONTH(created_at)", sql);
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
            double total_amount_after_tax = (double)(db.Reservations.Select(p => p.total_amount_after_tax).Sum());
            double total_amount_from_vendor = (double)(db.Reservations.Select(p => p.total_amount_from_vendor).Sum());
            double paid_amount = (double)(db.Reservations.Select(p => p.paid_amount).Sum());


            double profit = total_amount_after_tax - total_amount_from_vendor;
            double collected = paid_amount;
            double balance = total_amount_after_tax - paid_amount;
            
            double profitPercentage = (profit / total_amount_after_tax) * 100;
            double collectedPercentage = (collected / total_amount_after_tax) * 100;
            double balancePercentage = (balance / total_amount_after_tax) * 100;

            return Json(new { 
                xAxis = xAxis,
                yAxisCollected = yAxisCollected,
                yAxisBalance = yAxisBalance,
                yAxisProfit = yAxisProfit,
                profit = profit,
                collected = collected,
                balance = balance,
                profitPercentage = profitPercentage,
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