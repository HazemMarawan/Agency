using Agency.Helpers;
using Agency.Models;
using Agency.ViewModel;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
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

                total_amount += resDetail.amount;
                total_amount_after_tax += resDetail.amount_after_tax;
                total_amount_from_vendor += (vendor_room_price * (resDetail.no_of_days - 1));
                tax_amount += resDetail.tax;
                total_nights += resDetail.no_of_days - 1;
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

        public static Reservation UpdateTotals(int reservation_id, bool fromWebSite = false)
        {
            Reservation reservation = db.Reservations.Find(reservation_id);

            reservation.total_rooms = db.ReservationDetails.Where(rsd => rsd.reservation_id == reservation.id && reservation.is_canceled != 1 && rsd.id == rsd.parent_id).Count();
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
                MailServer mailServer = db.MailServers.Where(ms => ms.type == 1).FirstOrDefault();
                string confirmationMailPath = Path.Combine(HttpContext.Current.Server.MapPath("~/MailTemplates/itenrary.html"));
                StreamReader streamReader = new StreamReader(confirmationMailPath);
                string emailContent = streamReader.ReadToEnd();

                string Event = String.Empty;
                EventHotel eventHotel = db.EventHotels.Find(reservation.event_hotel_id);
                Event _event = db.Events.Find(eventHotel.event_id);
                if (_event.is_special == 1)
                {
                    Event = "Special";
                }
                else
                {
                    Event = _event.title;
                }

                Hotel hotel = db.Hotels.Find(eventHotel.hotel_id);
                HotelImage hotelImage = db.HotelImages.Where(hi => hi.hotel_id == hotel.id).FirstOrDefault();


                var resDetailData = (from resDetail in db.ReservationDetails
                                     join reserv in db.Reservations on resDetail.reservation_id equals reserv.id into rs
                                     from res in rs.DefaultIfEmpty()
                                     join cli in db.Clients on resDetail.client_id equals cli.id into cl
                                     from client in cl.DefaultIfEmpty()
                                     select new ReservationDetailViewModel
                                     {
                                         id = resDetail.id,
                                         amount = resDetail.amount,
                                         tax = resDetail.tax,
                                         room_type = resDetail.room_type,
                                         reservation_from = db.ReservationDetails.Where(rsd => rsd.parent_id == resDetail.id).Select(rsd => rsd.reservation_from).Min(),
                                         reservation_to = db.ReservationDetails.Where(rsd => rsd.parent_id == resDetail.id).Select(rsd => rsd.reservation_to).Max(),
                                         no_of_days = resDetail.no_of_days,
                                         parent_id = resDetail.parent_id,
                                         active = resDetail.active,
                                         client_id = resDetail.client_id,
                                         client_first_name = client.first_name,
                                         client_last_name = client.last_name,
                                         reservation_id = resDetail.reservation_id,
                                         currency = res.currency,
                                         string_reservation_from = db.ReservationDetails.Where(rsd => rsd.parent_id == resDetail.id).Select(rsd => rsd.reservation_from).Min().ToString(),
                                         string_reservation_to = db.ReservationDetails.Where(rsd => rsd.parent_id == resDetail.id).Select(rsd => rsd.reservation_to).Max().ToString(),
                                         vendor_code = resDetail.vendor_code,
                                         vendor_cost = db.ReservationDetails.Where(rsd => rsd.parent_id == resDetail.id).Select(rsd => rsd.vendor_cost).Sum(),
                                         room_price = resDetail.room_price,
                                         notify = resDetail.notify,
                                         is_transfered = resDetail.is_transfered,
                                         is_canceled = resDetail.is_canceled,
                                         paid_to_vendor = resDetail.paid_to_vendor,
                                         payment_to_vendor_deadline = (DateTime)resDetail.payment_to_vendor_deadline,
                                         string_payment_to_vendor_deadline = resDetail.payment_to_vendor_deadline.ToString(),
                                         string_spayment_to_vendor_notification_date = resDetail.payment_to_vendor_notification_date.ToString(),
                                         payment_to_vendor_notification_date = resDetail.payment_to_vendor_notification_date,
                                         paid_to_vendor_date = resDetail.paid_to_vendor_date,
                                         amount_paid_to_vendor = resDetail.amount_paid_to_vendor,
                                         cancelation_policy = resDetail.cancelation_policy,
                                         confirmation_id = resDetail.confirmation_id,
                                         guestReservations = db.ReservationDetails.Where(r => r.parent_id == resDetail.id && r.id != resDetail.id).Select(guestRes => new AdditionalReservationViewModel
                                         {
                                             id = guestRes.id,
                                             reservation_id = guestRes.reservation_id,
                                             parent_id = guestRes.parent_id,
                                             reservation_from = guestRes.reservation_from,
                                             reservation_to = guestRes.reservation_to,
                                             vendor_code = guestRes.vendor_code,
                                             vendor_cost = guestRes.vendor_cost,
                                             room_price = guestRes.room_price,
                                             string_reservation_from = guestRes.reservation_from.ToString(),
                                             string_reservation_to = guestRes.reservation_to.ToString(),
                                         }).ToList(),
                                     }).Where(d => d.reservation_id == reservation.id && d.parent_id == d.id).ToList();


                emailContent = emailContent.Replace("_reservation_id", reservation.id.ToString());
                emailContent = emailContent.Replace("_reservation_date", reservation.created_at.ToString().Split(' ')[0]);
                if(hotelImage != null)
                    emailContent = emailContent.Replace("_hotel_image", GetBaseUrl()+hotelImage.path);
                else
                    emailContent = emailContent.Replace("_hotel_image", "https://d15k2d11r6t6rl.cloudfront.net/public/users/Integrators/BeeProAgency/763737_747232/hotel-room-new-product-door-1330850.jpg");

                emailContent = emailContent.Replace("_hotel_name", hotel.name);
                emailContent = emailContent.Replace("_check_in", reservation.check_in.ToString().Split(' ')[0]);
                emailContent = emailContent.Replace("_check_out", reservation.check_out.ToString().Split(' ')[0]);
                emailContent = emailContent.Replace("_adult_counter", reservation.total_rooms.ToString());

                string Guests = String.Empty;
                int checkIfDataFound = 0;
                foreach(var client in resDetailData)
                {
                    if (!String.IsNullOrEmpty(client.client_first_name) || !String.IsNullOrEmpty(client.client_first_name))
                    {
                        Guests += @"<p style=""margin: 0;""><span style=""font-size:14px;"">" + client.client_first_name + " " + client.client_last_name + "</span></p>";
                        checkIfDataFound++;
                    }
                }

                if(checkIfDataFound != 0)
                    emailContent = emailContent.Replace("_guests", Guests);
                else
                    emailContent = emailContent.Replace("_guests", "Booked From System");

                emailContent = emailContent.Replace("_download_link", GetBaseUrl()+ "/Booking/itineraryPDF?reservation_id=" + reservation.id.ToString());

                ReservationService.sendMail(mailServer.outgoing_mail, reservation.reservations_officer_email, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);
                
                if(!String.IsNullOrEmpty(reservation.reservations_officer_email_2))
                    ReservationService.sendMail(mailServer.outgoing_mail, reservation.reservations_officer_email_2, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);
                
                if (!String.IsNullOrEmpty(reservation.reservations_officer_email_3))
                    ReservationService.sendMail(mailServer.outgoing_mail, reservation.reservations_officer_email_3, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);

                ReservationService.sendMail(mailServer.outgoing_mail, mailServer.incoming_mail, mailServer.title, emailContent, mailServer.outgoing_mail_server, mailServer.port.ToInt(), mailServer.outgoing_mail_password);
            }
            catch (Exception ex)
            {
                

                //}
            }
                return reservation;
            
        }

        public static void sendMail(string outgoing_mail, string email, string title, string body, string mail_server, int port, string password)
        {
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
                statisticsViewModel.collected_percentage = (statisticsViewModel.collected / statisticsViewModel.total_amount_after_tax_sum) * 100;

                statisticsViewModel.balance = db.Reservations.Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum() - statisticsViewModel.collected;
                statisticsViewModel.balance_percentage = (statisticsViewModel.balance / statisticsViewModel.total_amount_after_tax_sum) * 100;

                double? reservation_cancelation_fees = 0.0;
                List<Reservation> reservations = db.Reservations.Where(c => c.is_canceled == 1 && c.cancelation_fees != null).ToList();

                if (reservations.Count() > 0)
                {
                    reservation_cancelation_fees = db.Reservations.Where(c => c.is_canceled == 1 && c.cancelation_fees != null).Select(c => c.cancelation_fees).Sum();
                }

                statisticsViewModel.profit = db.Reservations.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum() - db.Reservations.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Where(t => t.is_canceled != 1).Select(t => t.total_amount_from_vendor).Sum();
                statisticsViewModel.profit += reservation_cancelation_fees;

                statisticsViewModel.total_amount_after_tax_sum_for_profit = db.Reservations.Where(r => r.paid_amount == r.total_amount_after_tax && r.paid_amount != 0 && r.is_canceled != 1).Where(t => t.is_canceled != 1).Select(t => t.total_amount_after_tax).Sum();
                statisticsViewModel.profit_percentage = (statisticsViewModel.profit / statisticsViewModel.total_amount_after_tax_sum_for_profit) * 100;

                statisticsViewModel.refund = db.Reservations.Where(t => t.is_canceled != 1).Select(t => t.refund).Sum();
                statisticsViewModel.refund_percentage = (statisticsViewModel.refund / statisticsViewModel.total_amount_after_tax_sum) * 100;

                statisticsViewModel.total_nights = db.Reservations.Where(t => t.is_canceled != 1).Select(n => n.total_nights).Sum();

            }
            return statisticsViewModel;
        }
        public static string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }
    }
}


