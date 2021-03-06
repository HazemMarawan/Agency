using Agency.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Agency.ViewModel
{
    public class ReservationViewModel
    {
        public int id { get; set; }
        public int? reservation_id { get; set; }
        public string welcome_message { get; set; }
        public string reservations_officer_name { get; set; }
        public string reservations_officer_phone { get; set; }
        public string reservations_officer_email { get; set; }
        public string reservations_officer_phone_2 { get; set; }
        public string reservations_officer_email_2 { get; set; }
        public string reservations_officer_phone_3 { get; set; }
        public string reservations_officer_email_3 { get; set; }
        public string company_fax { get; set; }
        public string company_address { get; set; }
        public string company_state { get; set; }
        public string company_postal_code { get; set; }
        public string company_country { get; set; }
        public int? payment_type { get; set; }
        public string credit_card_number { get; set; }
        public string security_code { get; set; }
        public string card_expiration_date { get; set; }
        public List<string> first_name { get; set; }
        public List<string> last_name { get; set; }
        public List<int> room_type { get; set; }
        public List<DateTime> reservation_from { get; set; }
        public List<DateTime> reservation_to { get; set; }
        public double? total_amount { get; set; }
        public double? total_amount_after_tax { get; set; }
        public double? total_amount_from_vendor { get; set; }
        public double? tax { get; set; }
        public double? tax_amount { get; set; }
        public double? credit { get; set; }
        public double? refund { get; set; }
        public string refund_id { get; set; }
        public double? advance_reservation_percentage { get; set; }
        public double? financial_advance { get; set; }
        public double? financial_due { get; set; }
        public DateTime? financial_advance_date { get; set; }
        public DateTime? financial_due_date { get; set; }
        public int? status { get; set; } //paid or partially
        public double? single_price { get; set; }
        public double? double_price { get; set; }
        public double? triple_price { get; set; }
        public double? quad_price { get; set; }
        public int? vendor_id { get; set; }
        public int? currency { get; set; }
        public string string_currency { get; set; }
        public string hotel_image { get; set; }
        public double? profit { get; set; }
        public int? is_special { get; set; }
        public int? hotel_id { get; set; }
        public int? active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }
        public int? deleted_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public int? company_id { get; set; }
        public Company Company { get; set; }
        public int? paid_to_vendor { get; set; }
        public int? event_hotel_id { get; set; }
        public double? hotel_rate { get; set; }
        public string company_name { get; set; }
        public string created_at_string { get; set; }
        public string updated_at_string { get; set; }
        public string financial_due_date_string { get; set; }
        public string hotel_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public double? paid_amount { get; set; }
        public int? opener { get; set; }
        public int? closer { get; set; }
        public double? vendor_single_price { get; set; }
        public double? vendor_douple_price { get; set; }
        public double? vendor_triple_price { get; set; }
        public double? vendor_quad_price { get; set; }
        public int? total_rooms { get; set; }
        public int? total_nights { get; set; }
        public DateTime? check_in { get; set; }
        public DateTime? check_out { get; set; }
        public DateTime balance_due_date { get; set; }
        public string string_balance_due_date { get; set; }
        public string string_check_in { get; set; }
        public string string_check_out { get; set; }
        public EventHotel EventHotel { get; set; }
        public string opener_name { get; set; }
        public string closer_name { get; set; }
        public int? shift { get; set; }
        public int? is_canceled { get; set; }
        public string shift_name { get; set; }
        public string event_name { get; set; }
        public double? event_tax { get; set; }
        public double? reservation_avg_price_before_tax { get; set; }
        public double? reservation_avg_price { get; set; }
        public double? vendor_avg_price { get; set; }
        public double? event_single_price { get; set; }
        public double? event_double_price { get; set; }
        public double? event_triple_price { get; set; }
        public double? event_quad_price { get; set; }
        public double? event_vendor_single_price { get; set; }
        public double? event_vendor_double_price { get; set; }
        public double? event_vendor_triple_price { get; set; }
        public double? event_vendor_quad_price { get; set; }
        public string vendor_code { get; set; }
        public double? total_price { get; set; }
        public string hotel_address { get; set; }
        public int single_rooms { get; set; }
        public int double_rooms { get; set; }
        public int triple_rooms { get; set; }
        public int quad_rooms { get; set; }
        public int? single_nights { get; set; }
        public int? double_nights { get; set; }
        public int? triple_nights { get; set; }
        public int? quad_nights { get; set; }
        public List<ReservationDetailViewModel> ReservationDetailVM { get; set; }
        public List<string> hotelFacilities { get; set; }
        public string created_by_name { get; set; }
        public string updated_by_name { get; set; }
        public string city_name { get; set; }
        public string location_name { get; set; }
        public double? vendor_cost { get; set; }
        public int countPaidToVendorRooms { get; set; }
        public int is_refund { get; set; }
        public int? countPaidToVendorNights { get; set; }
        public int event_id { get; set; }
        public string transaction_id { get; set; }
        public string hotel_name_special { get; set; }
        public double? cancelation_fees { get; set; }
        public double? transaction_fees { get; set; }
        public List<ReservationDetailViewModel> reservationDetails { get; set; }
        public List<ReservationCreditCardViewModel> reservationCreditCards { get; set; }
        public List<TransactionViewModel> transactions { get; set; }

        public void sendMail(string email,string message)//
        {
            //this.email
            MailMessage mail =
                 new MailMessage(
                     "agencysoftware@outlook.com",
                     email,
                     "Agency Confimation",
                     message
                     );
            mail.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com", 587);
            client.UseDefaultCredentials = true;

            NetworkCredential credentials = new NetworkCredential("agencysoftware@outlook.com", "P@ssw0rd@1234");

            client.Credentials = credentials;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.Send(mail);

        }

    }
}