using Agency.Helpers;
using Agency.Models;
using Agency.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
 
namespace Agency.Services
{
    public class ReservationService
    {
        public static AgencyDbContext db = new AgencyDbContext();
        public static ReservationViewModel calculateTotalandVendor(int res_id)
        {
            double? total_amount = 0;
            double? tax_amount = 0;
            double? total_amount_after_tax = 0;
            double? total_amount_from_vendor = 0;
            int? total_nights = 0;
            Reservation reservation = db.Reservations.Find(res_id);
            List<ReservationDetail> reservationDetails = db.ReservationDetails.Where(rd => rd.reservation_id == res_id).ToList();
            //Event event_obj = db.Events.Find(even_id);
            EventHotel eventHotel = db.EventHotels.Find(reservation.event_hotel_id);
            foreach (var resDetail in reservationDetails)
            {
                double? room_price = 0;
                double? vendor_room_price = resDetail.vendor_cost;
                if (resDetail.room_type == 1)
                {
                    room_price = reservation.single_price;
                }
                else if (resDetail.room_type == 2)
                {
                    room_price = reservation.double_price;
                }
                else if (resDetail.room_type == 3)
                {
                    room_price = reservation.triple_price;
                }
                else if (resDetail.room_type == 4)
                {
                    room_price = reservation.quad_price;
                }
                else
                {
                    room_price = reservation.single_price;
                }

                total_amount += resDetail.amount ;
                total_amount_after_tax += resDetail.amount_after_tax;
                total_amount_from_vendor += (vendor_room_price*(resDetail.no_of_days-1));
                tax_amount += resDetail.tax;
                total_nights += resDetail.no_of_days-1;
            }
            ReservationViewModel reservationViewModel = new ReservationViewModel();
            reservationViewModel.total_amount = total_amount;
            reservationViewModel.total_amount_after_tax = total_amount_after_tax;
            reservationViewModel.total_amount_from_vendor = total_amount_from_vendor;
            reservationViewModel.tax_amount = tax_amount;
            reservationViewModel.total_nights = total_nights;
            reservationViewModel.total_rooms = reservationDetails.Count();
            return reservationViewModel;
        }

        public static Reservation UpdateTotals(int reservation_id,bool fromWebSite=false)
        {
            Reservation reservation = db.Reservations.Find(reservation_id);

            reservation.total_rooms = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Count();
            reservation.total_amount = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.amount).Sum();
            reservation.total_amount_after_tax = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.amount_after_tax).Sum();
            reservation.tax_amount = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.tax).Sum();
            reservation.total_nights = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.no_of_days - 1).Sum();
            reservation.reservation_avg_price_before_tax = db.ReservationDetails.Where(resDet => resDet.reservation_id == reservation.id && reservation.is_canceled != 1).Select(resDet => resDet.amount).Sum() / reservation.total_nights;
            reservation.reservation_avg_price = db.ReservationDetails.Where(resDet => resDet.reservation_id == reservation.id && reservation.is_canceled != 1).Select(resDet => resDet.amount_after_tax).Sum() / reservation.total_nights;
            reservation.vendor_avg_price = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.vendor_cost).Sum() / reservation.total_nights;
            reservation.total_amount_from_vendor = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1).Select(s => s.vendor_cost).Sum();

            reservation.updated_at = DateTime.Now;
            db.SaveChanges();
            //if (fromWebSite)
            //{
            //reservation.updated_by = HttpContext.Current.Session["id"].ToString().ToInt();
            try
            { 
                string message = "<h1>Hello " + reservation.reservations_officer_name + "</h1>";
                message += "<h5><u>Reservation Information :</u></h5>";
                message += "<p>Email: " + reservation.reservations_officer_email + "</p>";
                message += "<p>Phone: " + reservation.reservations_officer_phone + "</p>";
                message += "<p>Hotel: " + reservation.hotel_name + "</p>";
                string checkIn = reservation.check_in.ToString();
                if (checkIn != null)
                {
                    checkIn = checkIn.Split(' ')[0];
                    message += "<p>Check In: " + checkIn + "</p>";

                }
                string checkOut = reservation.check_out.ToString();
                if (checkOut != null)
                {
                    checkOut = checkOut.Split(' ')[0];
                    message += "<p>Check Out: " + checkOut + "</p>";

                }
                message += "<p>Total Rooms: " + reservation.total_rooms + "</p>";
                message += "<p>Total Nights: " + reservation.total_nights + "</p>";
                message += "<p>Tax: " + Math.Ceiling((decimal)reservation.tax_amount).ToString() + "</p>";
                message += "<p>Cost: " + Math.Ceiling((decimal)reservation.total_amount).ToString() + "</p>";
                message += "<p>Cost After Tax: " +  Math.Ceiling((decimal)reservation.total_amount_after_tax).ToString() + "</p>";

                ReservationCreditCardViewModel reservationCreditCardViewModel = db.ReservationCreditCards.Where(s => s.reservation_id == reservation.id).Select(s => new ReservationCreditCardViewModel
                {
                    credit_card_number = s.credit_card_number,
                    security_code = s.security_code,
                    card_expiration_date = s.card_expiration_date

                }).FirstOrDefault();

                if(reservationCreditCardViewModel != null)
                {
                    if(!String.IsNullOrWhiteSpace(reservationCreditCardViewModel.credit_card_number))
                    {
                        if (reservationCreditCardViewModel.credit_card_number.Length >= 4)
                            message += "<p>Last Four Number From Card Number: " + reservationCreditCardViewModel.credit_card_number.Substring(reservationCreditCardViewModel.credit_card_number.Length - 4) + "</p>";
                    }
                }
                MailServer mailServer = db.MailServers.Where(ms => ms.type == 1).FirstOrDefault();

                ReservationService.sendMail(mailServer.outgoing_mail,reservation.reservations_officer_email,mailServer.title,mailServer.welcome_message.Replace("{mail_body}", message), mailServer.outgoing_mail_server,mailServer.port.ToInt(),mailServer.outgoing_mail_password);
                ReservationService.sendMail(mailServer.outgoing_mail,mailServer.incoming_mail,mailServer.title,mailServer.welcome_message.Replace("{mail_body}", message),mailServer.outgoing_mail_server,mailServer.port.ToInt(),mailServer.outgoing_mail_password);
            }
            catch(Exception ex)
            {

            }
            

            //}

            return reservation;
        }

        public static void sendMail(string outgoing_mail,string email,string title,string body,string mail_server,int port,string password)
        {
             //this.email
            MailMessage mail =
                 new MailMessage(
                     outgoing_mail,
                     email,
                     title,
                     body
                     );
            mail.IsBodyHtml = true;
            SmtpClient client = new SmtpClient(mail_server, port);
            client.UseDefaultCredentials = true;

            NetworkCredential credentials = new NetworkCredential(outgoing_mail, password);

            client.Credentials = credentials;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.Send(mail);

        }

        public static StatisticsViewModel calculateStatistics()
        {
            StatisticsViewModel statisticsViewModel = new StatisticsViewModel();
            var reservation = db.Reservations;
            if (reservation != null)
            {
                statisticsViewModel.total_amount_after_tax_sum = db.Reservations.Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum();
                statisticsViewModel.collected = db.Reservations.Where(t => t.is_canceled != 1).Select(t => t.paid_amount).Sum();
                statisticsViewModel.collected_percentage = (statisticsViewModel.collected / statisticsViewModel.total_amount_after_tax_sum)*100;

                statisticsViewModel.balance = db.Reservations.Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum() - statisticsViewModel.collected;
                statisticsViewModel.balance_percentage = (statisticsViewModel.balance / statisticsViewModel.total_amount_after_tax_sum)*100;

                double? reservation_cancelation_fees = 0.0;
                List<Reservation> reservations = db.Reservations.Where(c => c.is_canceled == 1 && c.cancelation_fees != null).ToList();

                if (reservations.Count() > 0)
                {
                    reservation_cancelation_fees = db.Reservations.Where(c => c.is_canceled == 1 && c.cancelation_fees != null).Select(c => c.cancelation_fees).Sum();
                }

                statisticsViewModel.profit = db.Reservations.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum() - db.Reservations.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Where(t => t.is_canceled != 1).Select(t => t.total_amount_from_vendor).Sum();
                statisticsViewModel.profit += reservation_cancelation_fees;

                statisticsViewModel.total_amount_after_tax_sum_for_profit = db.Reservations.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum();
                statisticsViewModel.profit_percentage = (statisticsViewModel.profit / statisticsViewModel.total_amount_after_tax_sum_for_profit) *100;

                statisticsViewModel.refund = db.Reservations.Where(t => t.is_canceled != 1).Select(t => t.refund).Sum();
                statisticsViewModel.refund_percentage = (statisticsViewModel.refund / statisticsViewModel.total_amount_after_tax_sum) * 100;
                
                statisticsViewModel.total_nights = db.Reservations.Where(t => t.is_canceled != 1).Select(n => n.total_nights).Sum();

            }
            return statisticsViewModel;
        }
    }
}