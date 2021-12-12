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
                hotels.Add(new HotelViewModel { total_nights = reader["total_nights"].ToString().ToInt(), 
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
            List<ReservationDetailViewModel> CheckinClients = (from resDetail in db.ReservationDetails
                                 join res in db.Reservations on resDetail.reservation_id equals res.id
                                 join client in db.Clients on resDetail.client_id equals client.id
                                 join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                                 select new ReservationDetailViewModel
                                 {
                                     id = resDetail.id,
                                     amount = resDetail.amount,
                                     tax = resDetail.tax,
                                     room_type = resDetail.room_type,
                                     reservation_from = resDetail.reservation_from,
                                     reservation_to = resDetail.reservation_to,
                                     no_of_days = resDetail.no_of_days,
                                     active = resDetail.active,
                                     client_id = resDetail.client_id,
                                     client_first_name = client.first_name,
                                     client_last_name = client.last_name,
                                     reservation_id = resDetail.reservation_id,
                                     currency = res.currency,
                                     string_reservation_from = resDetail.reservation_from.ToString(),
                                     string_reservation_to = resDetail.reservation_to.ToString(),
                                     vendor_code = resDetail.vendor_code,
                                     vendor_cost = resDetail.vendor_cost,
                                     notify = resDetail.notify,
                                     is_canceled = resDetail.is_canceled,
                                     paid_to_vendor = resDetail.paid_to_vendor,
                                     payment_to_vendor_deadline = resDetail.payment_to_vendor_deadline,
                                     payment_to_vendor_notification_date = resDetail.payment_to_vendor_notification_date,
                                     paid_to_vendor_date = resDetail.paid_to_vendor_date,
                                     amount_paid_to_vendor = resDetail.amount_paid_to_vendor,
                                     cancelation_policy = resDetail.cancelation_policy,
                                     confirmation_id = resDetail.confirmation_id,
                                     
                                 }).Where(r=>r.is_canceled == null && r.reservation_from.Year == nextWeekDate.Year && r.reservation_from.Month == nextWeekDate.Month && r.reservation_from.Day == nextWeekDate.Day).Take(10).ToList();

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

            List<ReservationViewModel> balanceDueDateReservations = (from res in db.Reservations
                           join company in db.Companies on res.company_id equals company.id
                           join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                           join even in db.Events on event_hotel.event_id equals even.id
                           join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                           join oU in db.Users on res.opener equals oU.id into us
                           from opener in us.DefaultIfEmpty()
                           join cU in db.Users on res.closer equals cU.id into use
                           from closer in use.DefaultIfEmpty()
                           select new ReservationViewModel
                           {
                               id = res.id,
                               total_amount = res.total_amount,
                               currency = res.currency,
                               tax = res.tax,
                               financial_advance = res.financial_advance,
                               financial_advance_date = res.financial_advance_date,
                               financial_due = res.financial_due,
                               financial_due_date = res.financial_due_date,
                               status = res.status,
                               single_price = res.single_price,
                               double_price = res.double_price,
                               triple_price = res.triple_price,
                               quad_price = res.quad_price,
                               vendor_single_price = res.vendor_single_price,
                               vendor_douple_price = res.vendor_douple_price,
                               vendor_triple_price = res.vendor_triple_price,
                               vendor_quad_price = res.vendor_quad_price,
                               vendor_id = res.vendor_id,
                               active = res.active,
                               company_name = company.name,
                               phone = company.phone,
                               email = company.email,
                               company_id = res.company_id,
                               event_hotel_id = event_hotel.id,
                               hotel_name = hotel.name,
                               hotel_rate = hotel.rate,
                               reservations_officer_name = res.reservations_officer_name,
                               reservations_officer_email = res.reservations_officer_email,
                               reservations_officer_phone = res.reservations_officer_phone,
                               opener = res.opener,
                               closer = res.closer,
                               opener_name = opener.full_name == null ? "No Opener Assigned" : opener.full_name,
                               closer_name = opener.full_name == null ? "No Closer Assigned" : closer.full_name,
                               check_in = res.check_in,
                               check_out = res.check_out,
                               string_check_in = res.check_in.ToString(),
                               string_check_out = res.check_out.ToString(),
                               total_nights = res.total_nights,
                               total_rooms = res.total_rooms,
                               profit = res.profit,
                               shift = res.shift,
                               created_at_string = res.created_at.ToString(),
                               event_name = even.title,
                               event_tax = even.tax,
                               event_single_price = event_hotel.single_price,
                               event_double_price = event_hotel.double_price,
                               event_triple_price = event_hotel.triple_price,
                               event_quad_price = event_hotel.quad_price,
                               event_vendor_single_price = event_hotel.vendor_single_price,
                               event_vendor_double_price = event_hotel.vendor_douple_price,
                               event_vendor_triple_price = event_hotel.vendor_triple_price,
                               event_vendor_quad_price = event_hotel.vendor_quad_price,
                               total_price = db.ReservationDetails.Where(r => r.reservation_id == res.id).Select(p => p.amount).Sum(),
                               paid_amount = res.paid_amount,
                               total_amount_after_tax = res.total_amount_after_tax,
                               total_amount_from_vendor = res.total_amount_from_vendor,
                               advance_reservation_percentage = res.advance_reservation_percentage,
                               tax_amount = res.tax_amount,
                               is_canceled = res.is_canceled,
                               balance_due_date = (DateTime)res.balance_due_date
                               //profit = calculateProfit(res.id).profit
                           }).Where(r => r.balance_due_date.Year == DateTime.Now.Year && r.balance_due_date.Month == DateTime.Now.Month && r.balance_due_date.Day == DateTime.Now.Day && r.is_canceled == null).Take(2).ToList();

            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            dashboardViewModel.topSellingHotels = hotels;
            dashboardViewModel.logs = reservationLog;
            dashboardViewModel.userData = userData;
            dashboardViewModel.LatestChats = LatestChats;
            dashboardViewModel.CheckinClients = CheckinClients;
            dashboardViewModel.currentUserTasks = currentUserTasks;
            dashboardViewModel.balanceDueDateReservations = balanceDueDateReservations;
            dashboardViewModel.TotalNotes = db.Notes.Where(n => n.created_by == currentUser.id).ToList().Count();
            return View(dashboardViewModel);
        }

        public JsonResult revenueReservations()
        {
            string cs = ConfigurationManager.ConnectionStrings["DBContextADO"].ConnectionString;

            SqlConnection sql = new SqlConnection(cs);
            sql.Open();

            SqlCommand comm = new SqlCommand(@"select SUM(paid_amount) as collected,SUM(total_amount_after_tax-paid_amount) as balance , SUM(total_amount_after_tax -total_amount_from_vendor) as profit,CONCAT( YEAR(created_at),'-' ,MONTH(created_at)) as date from Reservations where Reservations.is_canceled is null group by YEAR(created_at),MONTH(created_at)", sql);
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
            List<double?> total_amount_after_tax_reservations = db.Reservations.Where(p => p.is_canceled == null).Select(p => p.total_amount_after_tax).ToList();
            double? total_amount_after_tax = 0;
            if (total_amount_after_tax_reservations != null)
            {
                total_amount_after_tax = total_amount_after_tax_reservations.Sum();
            }

            List<double?> total_amount_from_vendor_reservations = db.Reservations.Where(p => p.is_canceled == null).Select(p => p.total_amount_from_vendor).ToList();
            double? total_amount_from_vendor = 0;
            if (total_amount_from_vendor_reservations != null)
            {
                total_amount_from_vendor = total_amount_from_vendor_reservations.Sum();
            }

            List<double?> paid_amount_reservations = db.Reservations.Where(p => p.is_canceled == null).Select(p => p.paid_amount).ToList();
            double? paid_amount = 0;
            if (paid_amount_reservations != null)
            {
                paid_amount = paid_amount_reservations.Sum();
            }

            List<double?> total_amount_from_vendor_finished_reservations = db.Reservations.Where(p => p.paid_amount == p.total_amount_after_tax && p.is_canceled == null).Select(p => p.total_amount_from_vendor).ToList();
            double? total_amount_from_vendor_finished = 0;
            if (total_amount_from_vendor_finished_reservations != null)
            {
                total_amount_from_vendor_finished = total_amount_from_vendor_finished_reservations.Sum();
            }


            List<double?> ttotal_amount_after_tax_finished_reservations = db.Reservations.Where(p => p.paid_amount == p.total_amount_after_tax && p.is_canceled == null).Select(p => p.total_amount_after_tax).ToList();
            double? total_amount_after_tax_finished = 0;
            if (total_amount_from_vendor_finished_reservations != null)
            {
                total_amount_after_tax_finished = ttotal_amount_after_tax_finished_reservations.Sum();
            }


            double? profit = total_amount_after_tax - total_amount_from_vendor;
            double? actualProfit = total_amount_after_tax_finished - total_amount_from_vendor_finished;

            double? collected = paid_amount;
            double? balance = total_amount_after_tax - paid_amount;
            
            double? profitPercentage = (profit / total_amount_after_tax) * 100;
            double? actualProfitPercentage = 0;
            if(actualProfit != 0 && total_amount_after_tax_finished != 0)
            {
                actualProfitPercentage = (actualProfit / total_amount_after_tax_finished) * 100;
            }

            double? collectedPercentage = (collected / total_amount_after_tax) * 100;
            double? balancePercentage = (balance / total_amount_after_tax) * 100;

            return Json(new { 
                xAxis = xAxis,
                yAxisCollected = yAxisCollected,
                yAxisBalance = yAxisBalance,
                yAxisProfit = yAxisProfit,
                profit = profit,
                actualProfit = actualProfit,
                collected = collected,
                balance = balance,
                profitPercentage = profitPercentage,
                actualProfitPercentage = actualProfitPercentage,
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