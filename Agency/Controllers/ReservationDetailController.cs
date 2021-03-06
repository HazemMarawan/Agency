using Agency.Auth;
using Agency.Enums;
using Agency.Helpers;
using Agency.Models;
using Agency.Services;
using Agency.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agency.Controllers
{
    [CustomAuthenticationFilter]
    public class ReservationDetailController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();

        // GET: ReservationDetail
        public ActionResult Index(int? id)
        {
            if (Request.IsAjaxRequest())
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                // Getting all data    
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
                                   string_reservation_from = db.ReservationDetails.Where(rsd=>rsd.parent_id == resDetail.id).Select(rsd=>rsd.reservation_from).Min().ToString(),
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
                               }).Where(d => d.reservation_id == id && d.parent_id == d.id);

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    resDetailData = resDetailData.Where(m => m.client_first_name.ToLower().Contains(searchValue.ToLower()) || m.amount.ToString().ToLower().Contains(searchValue.ToLower()) ||
                     m.client_last_name.ToLower().Contains(searchValue.ToLower()) || m.no_of_days.ToString().ToLower().Contains(searchValue.ToLower()));
                }

                //total number of rows count     
                var displayResult = resDetailData.OrderByDescending(u => u.id).Skip(skip)
                     .Take(pageSize).ToList();
                var totalRecords = resDetailData.Count();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = displayResult

                }, JsonRequestBehavior.AllowGet);

            }

            ViewBag.id = id;

            return View();
        }
        public ActionResult getAll()
        {
            if (Request.IsAjaxRequest())
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                // Getting all data    
                var resDetailData = (from resDetail in db.ReservationDetails
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

                                     });
                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    resDetailData = resDetailData.Where(m => m.client_first_name.ToLower().Contains(searchValue.ToLower()) || m.amount.ToString().ToLower().Contains(searchValue.ToLower()) ||
                     m.client_last_name.ToLower().Contains(searchValue.ToLower()) || m.no_of_days.ToString().ToLower().Contains(searchValue.ToLower()));
                }

                //total number of rows count     
                var displayResult = resDetailData.OrderByDescending(u => u.id).Skip(skip)
                     .Take(pageSize).ToList();
                var totalRecords = resDetailData.Count();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = displayResult

                }, JsonRequestBehavior.AllowGet);

            }
           
            return View();
        }

        public JsonResult saveReservation(ReservationDetailViewModel detailViewModel)
        {
            if(detailViewModel.id == 0)
            {
                ReservationDetail detail = AutoMapper.Mapper.Map<ReservationDetailViewModel, ReservationDetail>(detailViewModel);

                Reservation reservation = db.Reservations.Find(detail.reservation_id);

                EventHotel eventHotel = db.EventHotels.Find(reservation.event_hotel_id);

                Event event_row = db.Events.Find(eventHotel.event_id);

                //double? room_price = 0;
                //double? vendor_room_price = 0;

                //if (detailViewModel.room_type == 1)
                //{
                //    room_price = reservation.single_price;
                //    vendor_room_price = reservation.vendor_single_price;
                //}
                //else if (detailViewModel.room_type == 2)
                //{
                //    room_price = reservation.double_price;
                //    vendor_room_price = reservation.vendor_douple_price;
                //}
                //else if (detailViewModel.room_type == 3)
                //{
                //    room_price = reservation.triple_price;
                //    vendor_room_price = reservation.vendor_triple_price;
                //}
                //else if (detailViewModel.room_type == 4)
                //{
                //    room_price = reservation.quad_price;
                //    vendor_room_price = reservation.vendor_quad_price;
                //}
                //else
                //{
                //    vendor_room_price = reservation.vendor_single_price;
                //}
                int Days = (detailViewModel.reservation_to - detailViewModel.reservation_from).Days;
                detail.no_of_days = Days + 1;
                var amount = (detailViewModel.room_price * Days); 
                //var vendor_amount = (vendor_room_price * Days) ;
                detail.tax = amount * (reservation.tax/100);
                detail.amount = amount;
                detail.amount_after_tax = amount + detail.tax;
                detail.created_at = DateTime.Now;
                detail.created_by = Session["id"].ToString().ToInt();
                detail.active = 1;
                db.ReservationDetails.Add(detail);
                db.SaveChanges();

                detail.parent_id = detail.id;
                db.SaveChanges();

                Client client = new Client();
                client.first_name = detailViewModel.client_first_name;
                client.last_name = detailViewModel.client_last_name;
                client.company_id = reservation.company_id;
                client.active = 1;
                client.created_at = DateTime.Now;
                client.created_by = Session["id"].ToString().ToInt();
                db.Clients.Add(client);
                db.SaveChanges();

                List<ReservationDetail> reservationDetails = db.ReservationDetails.Where(r => r.reservation_id == detailViewModel.reservation_id).ToList();
                if (reservationDetails.Count() != 0)
                {
                    DateTime minDate = reservationDetails.Select(s => s.reservation_from).Min();
                    DateTime maxDate = reservationDetails.Select(s => s.reservation_to).Max();

                    reservation.check_in = minDate;
                    reservation.check_out = maxDate;
                    reservation.balance_due_date = Convert.ToDateTime(reservation.check_in).AddDays(-30);
                    db.SaveChanges();
                }

                ReservationService.UpdateTotals(reservation.id);

                detail.client_id = client.id;
                db.SaveChanges();

                Logs.ReservationActionLog(Session["id"].ToString().ToInt(), detail.reservation_id, "Add", "Add Reservation #" + detail.id);

            }
            else
            {
                ReservationDetail detail = db.ReservationDetails.Find(detailViewModel.id);

                Reservation reservation = db.Reservations.Find(detail.reservation_id);

                EventHotel eventHotel = db.EventHotels.Find(reservation.event_hotel_id);

                string editHtml = "";

                Event event_row = db.Events.Find(eventHotel.event_id);
                if (detail.reservation_from != detailViewModel.reservation_from)
                    editHtml += "From: " + detail.reservation_from.ToString().Split(' ')[0] + @" | <span style=""color:red;"">" + detailViewModel.reservation_from.ToString().Split(' ')[0] + "</span><br>";
                detail.reservation_from = detailViewModel.reservation_from;

                if (detail.reservation_to != detailViewModel.reservation_to)
                    editHtml += "To: " + detail.reservation_to.ToString().Split(' ')[0] + @" | <span style = ""color:red;"">" + detailViewModel.reservation_to.ToString().Split(' ')[0] + "</span><br>";

                detail.reservation_to = detailViewModel.reservation_to;

                if(detail.room_type!= null)
                { 
                if (detail.room_type != detailViewModel.room_type)
                    editHtml += "Room Type: " + ((RoomType)detail.room_type).ToString() + @" | <span style = ""color:red;"">" + ((RoomType)detailViewModel.room_type).ToString() + "</span><br>";
                }
                else
                {
                    if(detailViewModel.room_type != null)
                        editHtml += @"Room Type: <span style = ""color:red;"">" + ((RoomType)detailViewModel.room_type).ToString() + "</span><br>";

                }
                detail.room_type = detailViewModel.room_type;

                if (detail.room_price != null)
                {
                    if (detail.room_price != detailViewModel.room_price)
                        editHtml += "Room Price: " + detail.room_price.ToString() + @" | <span style=""color: red;"">" + detailViewModel.room_price.ToString() + "</span><br>";
                }
                else
                {
                    if(detailViewModel.room_price != null)
                        editHtml += @"Room Price: <span style=""color: red;"">" + detailViewModel.room_price.ToString() + "</span><br>";
                }
                detail.room_price = detailViewModel.room_price;

                
                if(detail.vendor_code != null)
                { 
                    if (detail.vendor_code != detailViewModel.vendor_code)
                        editHtml += "Vendor Code: " + detail.vendor_code + @" | <span style = ""color:red; "">" + detailViewModel.vendor_code + "</span><br>";
                }
                else
                {
                    if(detailViewModel.vendor_code != null)
                        editHtml += @"Vendor Code: <span style = ""color:red; "">" + detailViewModel.vendor_code + "</span><br>";
                }
                detail.vendor_code = detailViewModel.vendor_code;

                if(detail.vendor_cost != null)
                { 
                    if (detail.vendor_cost != detailViewModel.vendor_cost)
                        editHtml += "Total Vendor: " + detail.vendor_cost + @" | <span style =""color:red; "">" + detailViewModel.vendor_cost + "</span><br>";
                }
                else
                {
                    if(detailViewModel.vendor_cost != null)
                        editHtml += @"Total Vendor: <span style =""color:red; "">" + detailViewModel.vendor_cost + "</span><br>";
                }
                detail.vendor_cost = detailViewModel.vendor_cost;

                if(detail.cancelation_policy !=null)
                { 
                    if (detail.cancelation_policy != detailViewModel.cancelation_policy)
                        editHtml += "Cancelation Policy: " + detail.cancelation_policy + @" | <span style=""color:red;"">" + detailViewModel.cancelation_policy + "</span><br>";
                }
                else
                {
                    if(detailViewModel.cancelation_policy != null)
                        editHtml += @"Cancelation Policy: <span style=""color:red;"">" + detailViewModel.cancelation_policy + "</span><br>";
                }
                detail.cancelation_policy = detailViewModel.cancelation_policy;

              
                detail.notify = detailViewModel.notify;

                if (detail.paid_to_vendor_date != null)
                {
                    if (detail.paid_to_vendor_date != detailViewModel.paid_to_vendor_date)
                        editHtml += "Paid To Vendor Date: " + detail.paid_to_vendor_date.ToString().Split(' ')[0] + @" | <span style=""color: red;"">" + detailViewModel.paid_to_vendor_date.ToString().Split(' ')[0] + "</span><br>";
                }
                else
                {
                    if(detailViewModel.paid_to_vendor_date != null)
                        editHtml += @"Paid To Vendor Date: <span style=""color: red;"">" + detailViewModel.paid_to_vendor_date.ToString().Split(' ')[0] + "</span><br>";
                }
                detail.payment_to_vendor_deadline = detailViewModel.payment_to_vendor_deadline;

                if(detail.paid_to_vendor!=null)
                { 
                    if (detail.paid_to_vendor != detailViewModel.paid_to_vendor)
                        editHtml += "Paid To Vendor: " + (detail.paid_to_vendor == 1?"Yes":"No") + @" | <span style=""color: red;"">" + (detailViewModel.paid_to_vendor == 1 ? "Yes" : "No") + "</span><br>";
                }
                else
                {
                    if(detailViewModel.paid_to_vendor != null)
                        editHtml += @"Paid To Vendor: <span style=""color: red;"">" + (detailViewModel.paid_to_vendor == 1 ? "Yes" : "No") + "</span><br>";
                }
                detail.paid_to_vendor = detailViewModel.paid_to_vendor;

                if(detail.amount_paid_to_vendor != null)
                { 
                    if (detail.amount_paid_to_vendor != detailViewModel.amount_paid_to_vendor)
                        editHtml += "Amount Paid To Vendor: " + detail.amount_paid_to_vendor.ToString()+ @" | <span style=""color: red;"">" + detailViewModel.amount_paid_to_vendor.ToString() + "</span><br>";
                }
                else
                {
                    if(detailViewModel.amount_paid_to_vendor !=null)
                        editHtml += @"Amount Paid To Vendor: <span style=""color: red;"">" + detailViewModel.amount_paid_to_vendor.ToString() + "</span><br>";
                }
                detail.amount_paid_to_vendor = detailViewModel.amount_paid_to_vendor;

                if(detail.payment_to_vendor_notification_date != null)
                { 
                    if (detail.payment_to_vendor_notification_date != detailViewModel.payment_to_vendor_notification_date)
                        editHtml += "Payment To Vendor Notification Date: " + detail.payment_to_vendor_notification_date.ToString().Split(' ')[0] + @" | <span style=""color: red;"">" + detailViewModel.payment_to_vendor_notification_date.ToString().Split(' ')[0] + "</span><br>";
                }
                else
                {
                    if (detailViewModel.payment_to_vendor_notification_date != null)
                        editHtml += @"Payment To Vendor Notification Date: <span style=""color: red;"">" + detailViewModel.payment_to_vendor_notification_date.ToString().Split(' ')[0] + "</span><br>";
                }
                detail.payment_to_vendor_notification_date = detailViewModel.payment_to_vendor_notification_date;
                
                detail.is_canceled = detailViewModel.is_canceled;

                if(detail.confirmation_id != null)
                { 
                    if (detail.confirmation_id != detailViewModel.confirmation_id)
                        editHtml += "Confirmation Id: " + detail.confirmation_id + @" | <span style=""color: red;"">" + detailViewModel.confirmation_id + "</span><br>";
                }
                else
                {
                    if(detailViewModel.confirmation_id != null)
                        editHtml += @"Confirmation Id: <span style=""color: red;"">" + detailViewModel.confirmation_id + "</span><br>";
                }
                detail.confirmation_id = detailViewModel.confirmation_id;

                //double? room_price = 0;
                //double? vendor_room_price = detailViewModel.vendor_cost;

                //if (detailViewModel.room_type == 1)
                //{
                //    room_price = reservation.single_price;
                //}
                //else if(detailViewModel.room_type == 2)
                //{
                //    room_price = reservation.double_price;
                //}
                //else if (detailViewModel.room_type == 3)
                //{
                //    room_price = reservation.triple_price;
                //}
                //else if (detailViewModel.room_type == 4)
                //{
                //    room_price = reservation.quad_price;
                //}
                //else
                //{ 
                //    //default single
                //    room_price = reservation.single_price;
                //}

                int Days = (detailViewModel.reservation_to - detailViewModel.reservation_from).Days;// + 1;
                detail.no_of_days = Days+1;
                var amount = (detailViewModel.room_price * Days); //* (100 - reservation.advance_reservation_percentage)) / 100;  //event_row
                //var vendor_amount = (detailViewModel.vendor_room_price * Days);//* (100 - reservation.advance_reservation_percentage)) / 100;  //event_row
                detail.tax = amount * (reservation.tax / 100);
                detail.amount = amount;
                detail.amount_after_tax = amount + detail.tax;
                
                detail.updated_at = DateTime.Now;
                detail.updated_by = Session["id"].ToString().ToInt();
                db.SaveChanges();

                if (detail.client_id != null)
                {
                    Client client = db.Clients.Find(detail.client_id);
                    if (client.first_name != detailViewModel.client_first_name)
                        editHtml += "First Name: " + client.first_name + @" | <span style=""color: red;"">" + detailViewModel.client_first_name + "</span><br>";

                    client.first_name = detailViewModel.client_first_name;

                    if (client.last_name != detailViewModel.client_last_name)
                        editHtml += "Last Name: " + client.last_name + @" | <span style=""color: red;"">" + detailViewModel.client_last_name + "</span><br>";

                    client.last_name = detailViewModel.client_last_name;
                    client.updated_at = DateTime.Now;
                    client.updated_by = Session["id"].ToString().ToInt();
                    db.SaveChanges();
                }
                else
                {
                    Client client = new Client();
                        editHtml += @" | <span style=""color: red;"">" + detailViewModel.client_first_name + "</span><br>";

                    client.first_name = detailViewModel.client_first_name;

                        editHtml +=@" | <span style=""color: red;"">" + detailViewModel.client_last_name + "</span><br>";

                    client.last_name = detailViewModel.client_last_name;
                    client.updated_at = DateTime.Now;
                    client.updated_by = Session["id"].ToString().ToInt();

                    db.Clients.Add(client);
                    db.SaveChanges();

                    detail.client_id = client.id;
                    db.SaveChanges();
                }

                if(detailViewModel.roomPrices != null)
                {
                    for(var resDet = 0; resDet < detailViewModel.roomPrices.Count; resDet++)
                    {
                        ReservationDetail additionalReservationDetails = db.ReservationDetails.Find(detailViewModel.ids[resDet]);
                        additionalReservationDetails.reservation_id = detailViewModel.reservationIds[resDet];
                        additionalReservationDetails.parent_id = detailViewModel.parentIds[resDet];
                        additionalReservationDetails.vendor_code = detailViewModel.vendorCodes[resDet];
                        additionalReservationDetails.vendor_cost = detailViewModel.vendorCosts[resDet];
                        additionalReservationDetails.room_price = detailViewModel.roomPrices[resDet];
                        additionalReservationDetails.reservation_from = detailViewModel.reservationsFrom[resDet];
                        additionalReservationDetails.reservation_to = detailViewModel.reservationsTo[resDet];
                        db.SaveChanges();
                    }
                }

                List<ReservationDetail> reservationDetails = db.ReservationDetails.Where(r => r.reservation_id == detailViewModel.reservation_id && reservation.is_canceled != 1).ToList();
                if (reservationDetails.Count() != 0)
                {
                    DateTime minDate = reservationDetails.Select(s => s.reservation_from).Min();
                    DateTime maxDate = reservationDetails.Select(s => s.reservation_to).Max();

                    reservation.check_in = minDate;
                    reservation.check_out = maxDate;
                    reservation.balance_due_date = Convert.ToDateTime(reservation.check_in).AddDays(-30);
                    db.SaveChanges();
                }

                ReservationService.UpdateTotals(reservation.id);
                //here log
                Logs.ReservationActionLog(Session["id"].ToString().ToInt(), detail.reservation_id, "Edit", editHtml);

            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult TransferReservation(ReservationDetailViewModel detailViewModel)
        {
            Reservation reservation = db.Reservations.Find(detailViewModel.reservation_id);
            if(reservation != null)
            { 
                ReservationDetail transferedReservation = db.ReservationDetails.Find(detailViewModel.id);
                transferedReservation.reservation_id = detailViewModel.reservation_id;
                transferedReservation.is_transfered = 1;
                //db.SaveChanges();

                Client client = db.Clients.Find(transferedReservation.client_id);
                client.first_name = detailViewModel.client_first_name;
                client.last_name = detailViewModel.client_last_name;
                db.SaveChanges();

                return Json(new { done = 1 }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { done = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult getComments(int? id)
        {
            if (Request.IsAjaxRequest())
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                // Getting all data    
                var resDetailData = (from comm in db.ReservationComments
                                     join comment_by in db.Users on comm.created_by equals comment_by.id
                                     select new ReservationCommentViewModel
                                     {
                                         id = comm.id,
                                         comment = comm.comment,
                                         created_by_string = comment_by.full_name,
                                         created_at_string = comm.created_at.ToString(),
                                         reservation_id = comm.reservation_id
                                     }).Where(r => r.reservation_id == id);

                //Search    
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    resDetailData = resDetailData.Where(m => m.client_first_name.ToLower().Contains(searchValue.ToLower()) || m.amount.ToString().ToLower().Contains(searchValue.ToLower()) ||
                //     m.client_last_name.ToLower().Contains(searchValue.ToLower()) || m.no_of_days.ToString().ToLower().Contains(searchValue.ToLower()));
                //}

                //total number of rows count     
                var displayResult = resDetailData.OrderByDescending(u => u.id).Skip(skip)
                     .Take(pageSize).ToList();
                var totalRecords = resDetailData.Count();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = displayResult

                }, JsonRequestBehavior.AllowGet);

            }
            ViewBag.id = id;

            return View();
        }

        public JsonResult saveComment(ReservationCommentViewModel RCommentViewModel)
        {
            if (RCommentViewModel.id == 0)
            {
                ReservationComment rComm = AutoMapper.Mapper.Map<ReservationCommentViewModel, ReservationComment>(RCommentViewModel);

                rComm.created_at = DateTime.Now;
                rComm.created_by = Session["id"].ToString().ToInt();
                rComm.active = 1;
                db.ReservationComments.Add(rComm);
                db.SaveChanges();
                Logs.ReservationActionLog(Session["id"].ToString().ToInt(), rComm.reservation_id, "Add", "Add Comment #" + rComm.id);

            }
            else
            {
                ReservationComment rCommOld = db.ReservationComments.Find(RCommentViewModel.id);
                rCommOld.comment = RCommentViewModel.comment;
                rCommOld.updated_at = DateTime.Now;
                rCommOld.updated_by = Session["id"].ToString().ToInt();
                db.SaveChanges();

                Logs.ReservationActionLog(Session["id"].ToString().ToInt(), rCommOld.reservation_id, "Edit", "Edit Comment #" + rCommOld.id);

            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult getTasks(int? id)
        {
            if (Request.IsAjaxRequest())
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                // Getting all data    
                var resDetailData = (from task in db.ReservationTasks
                                     join assigned_by in db.Users on task.created_by equals assigned_by.id
                                     join assigned_to in db.Users on task.task_to_user equals assigned_to.id
                                     select new ReservationTaskViewModel
                                     {
                                         id = task.id,
                                         task_title = task.task_title,
                                         task_detail = task.task_detail,
                                         notification_date = task.notification_date,
                                         task_to_user = task.task_to_user,
                                         due_date = task.due_date,
                                         status = task.status,
                                         created_by_string = assigned_by.full_name,
                                         task_to_user_string = assigned_to.full_name,
                                         created_at_string = task.created_at.ToString(),
                                         reservation_id = task.reservation_id
                                     }).Where(r => r.reservation_id == id);

                //Search    
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    resDetailData = resDetailData.Where(m => m.client_first_name.ToLower().Contains(searchValue.ToLower()) || m.amount.ToString().ToLower().Contains(searchValue.ToLower()) ||
                //     m.client_last_name.ToLower().Contains(searchValue.ToLower()) || m.no_of_days.ToString().ToLower().Contains(searchValue.ToLower()));
                //}

                //total number of rows count     
                var displayResult = resDetailData.OrderByDescending(u => u.id).Skip(skip)
                     .Take(pageSize).ToList();
                var totalRecords = resDetailData.Count();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = displayResult

                }, JsonRequestBehavior.AllowGet);

            }
            ViewBag.id = id;
            ViewBag.Users = db.Users.Select(s => new { s.id, s.full_name }).ToList();

            return View();
        }
        public ActionResult getLogs(int? id)
        {
            if (Request.IsAjaxRequest())
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                // Getting all data    
                var resDetailData = (from log in db.ReservationLogs
                                     join user in db.Users on log.user_id equals user.id
                                     select new ReservationLogViewModel
                                     {
                                         id = log.id,
                                         full_name = user.full_name,
                                         string_created_at = log.created_at.ToString(),
                                         action = log.action,
                                         description = log.description,
                                         reservation_id = log.reservation_id
                                     }).Where(r => r.reservation_id == id);


                //total number of rows count     
                var displayResult = resDetailData.OrderByDescending(u => u.id).Skip(skip)
                     .Take(pageSize).ToList();
                var totalRecords = resDetailData.Count();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = displayResult

                }, JsonRequestBehavior.AllowGet);

            }
            return View();
        }

        public JsonResult saveTask(ReservationTaskViewModel RTaskViewModel)
        {
            if (RTaskViewModel.id == 0)
            {
                ReservationTask reservationTask = AutoMapper.Mapper.Map<ReservationTaskViewModel, ReservationTask>(RTaskViewModel);

                reservationTask.created_at = DateTime.Now;
                reservationTask.created_by = Session["id"].ToString().ToInt();
                reservationTask.active = 1;
                db.ReservationTasks.Add(reservationTask);
                db.SaveChanges();

                Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservationTask.reservation_id, "Add", "Add Task #" + reservationTask.id);

            }
            else
            {
                ReservationTask reservationTaskOld = db.ReservationTasks.Find(RTaskViewModel.id);
                reservationTaskOld.task_title = RTaskViewModel.task_title;
                reservationTaskOld.task_detail = RTaskViewModel.task_detail;
                reservationTaskOld.notification_date = RTaskViewModel.notification_date;
                reservationTaskOld.due_date = RTaskViewModel.due_date;
                reservationTaskOld.task_to_user = RTaskViewModel.task_to_user;
                reservationTaskOld.status = RTaskViewModel.status;
                reservationTaskOld.updated_at = DateTime.Now;
                reservationTaskOld.updated_by = Session["id"].ToString().ToInt();
                db.SaveChanges();

                Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservationTaskOld.reservation_id, "Edit", "Edit Task #" + reservationTaskOld.id);

            }

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult deleteReservation(int id)
        {
            ReservationDetail detail = db.ReservationDetails.Find(id);
            int? reservationId = detail.reservation_id;

            Reservation reservation = db.Reservations.Find(detail.reservation_id);
           
            double? amount = detail.amount;
            double? tax = detail.tax;

            db.ReservationDetails.Remove(detail);
            db.SaveChanges();

            ReservationViewModel updatedTotals = ReservationService.calculateTotalandVendor(reservation.id);

            reservation.total_amount = updatedTotals.total_amount;
            reservation.total_amount_after_tax = updatedTotals.total_amount_after_tax;
            reservation.total_amount_from_vendor = updatedTotals.total_amount_from_vendor;
            reservation.tax_amount = updatedTotals.tax_amount;
            reservation.total_nights = updatedTotals.total_nights;
           
            reservation.updated_at = DateTime.Now;
            reservation.updated_by = Session["id"].ToString().ToInt();
            db.SaveChanges();
            Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservationId, "Delete", "Delete Reservation #" + id);

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult deleteComment(int id)
        {
            ReservationComment comment = db.ReservationComments.Find(id);
            int? reservationId = comment.reservation_id;
            db.ReservationComments.Remove(comment);
            db.SaveChanges();

            Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservationId, "Delete", "Delete Comment #" + id);

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult deleteTask(int id)
        {
            ReservationTask task = db.ReservationTasks.Find(id);
            int? reservationId = task.reservation_id;
            db.ReservationTasks.Remove(task);
            db.SaveChanges();

            Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservationId, "Delete", "Delete Task #" + id);

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult cancelReservation(int id)
        {
            ReservationDetail reservationDetail = db.ReservationDetails.Find(id);
            reservationDetail.is_canceled = 1;
            db.SaveChanges();

            Logs.ReservationActionLog(Session["id"].ToString().ToInt(), reservationDetail.reservation_id, "Cancel", "Delete Reservation #" + id);

            return Json(new { message = "done" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Itinirary(int id)
        {
            Logs.ReservationActionLog(Session["id"].ToString().ToInt(), id, "Export", "Export Itinirary #" + id);

            ReservationViewModel resData = (from res in db.Reservations
                           join company in db.Companies on res.company_id equals company.id
                           join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                           join even in db.Events on event_hotel.event_id equals even.id
                           join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                           //join locationHotel in db.HotelLocations on hotel.id equals locationHotel.hotel_id
                           //join location in db.Locations on locationHotel.location_id equals location.id
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
                               created_at = res.created_at,
                               company_name = company.name,
                               phone = company.phone,
                               email = company.email,
                               company_id = res.company_id,
                               event_hotel_id = event_hotel.id,
                               hotel_id = hotel.id,
                               hotel_name = hotel.name,
                               hotel_rate = hotel.rate,
                               hotel_address = hotel.address,
                               reservations_officer_name = res.reservations_officer_name,
                               reservations_officer_email = res.reservations_officer_email,
                               reservations_officer_phone = res.reservations_officer_phone,
                               opener = res.opener,
                               closer = res.closer,
                               check_in = res.check_in,
                               check_out = res.check_out,
                               string_check_in = res.check_in.ToString(),
                               string_check_out = res.check_out.ToString(),
                               //total_nights = res.total_nights,
                               //total_rooms = res.total_rooms,
                               profit = res.profit,
                               shift = res.shift,
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
                               total_rooms = db.ReservationDetails.Where(tr => tr.reservation_id == id).ToList().Count(),
                               total_nights = db.ReservationDetails.Where(tr => tr.reservation_id == id).Select(tn => tn.no_of_days).Sum(),
                               hotelFacilities = db.HotelFacilities.Where(f => f.hotel_id == hotel.id).Select(fh => fh.name).ToList(),
                               //profit = calculateProfit(res.id).profit,
                               ReservationDetailVM = (from resDet in db.ReservationDetails
                                                    join client in db.Clients on resDet.client_id equals client.id
                                                    select new ReservationDetailViewModel
                                                    {
                                                        res_id = resDet.id,
                                                        reservation_from = resDet.reservation_from,
                                                        reservation_to = resDet.reservation_to,
                                                        room_type = resDet.room_type,
                                                        client_first_name = client.first_name,
                                                        client_last_name = client.last_name,

                                                    }).Where(rd => rd.res_id == id).ToList()
                           }).Where(r => r.id == id).FirstOrDefault();
            var report = new Rotativa.ViewAsPdf("Itinirary", resData);
            return report;
        }

        public ActionResult Invoice(int id)
        {
            Logs.ReservationActionLog(Session["id"].ToString().ToInt(), id, "Export", "Export Invoice #" + id);

            ReservationViewModel resData = (from res in db.Reservations
                                            join company in db.Companies on res.company_id equals company.id
                                            join event_hotel in db.EventHotels on res.event_hotel_id equals event_hotel.id
                                            join even in db.Events on event_hotel.event_id equals even.id
                                            join hotel in db.Hotels on event_hotel.hotel_id equals hotel.id
                                            //join locationHotel in db.HotelLocations on hotel.id equals locationHotel.hotel_id
                                            //join location in db.Locations on locationHotel.location_id equals location.id
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
                                                created_at = res.created_at,
                                                company_name = company.name,
                                                phone = company.phone,
                                                email = company.email,
                                                company_id = res.company_id,
                                                event_hotel_id = event_hotel.id,
                                                hotel_id = hotel.id,
                                                hotel_name = hotel.name,
                                                hotel_rate = hotel.rate,
                                                hotel_address = hotel.address,
                                                reservations_officer_name = res.reservations_officer_name,
                                                reservations_officer_email = res.reservations_officer_email,
                                                reservations_officer_phone = res.reservations_officer_phone,
                                                opener = res.opener,
                                                closer = res.closer,
                                                check_in = res.check_in,
                                                check_out = res.check_out,
                                                string_check_in = res.check_in.ToString(),
                                                string_check_out = res.check_out.ToString(),
                                                //total_nights = res.total_nights,
                                                //total_rooms = res.total_rooms,
                                                profit = res.profit,
                                                shift = res.shift,
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
                                                total_rooms = db.ReservationDetails.Where(tr => tr.reservation_id == id).ToList().Count(),
                                                total_nights = db.ReservationDetails.Where(tr => tr.reservation_id == id).Select(tn => tn.no_of_days).Sum(),
                                                hotelFacilities = db.HotelFacilities.Where(f => f.hotel_id == hotel.id).Select(fh => fh.name).ToList(),
                                                single_rooms = db.ReservationDetails.Where(sr => sr.room_type == 1 && sr.reservation_id == id).Count(),
                                                double_rooms = db.ReservationDetails.Where(sr => sr.room_type == 2 && sr.reservation_id == id).Count(),
                                                triple_rooms = db.ReservationDetails.Where(sr => sr.room_type == 3 && sr.reservation_id == id).Count(),
                                                quad_rooms = db.ReservationDetails.Where(sr => sr.room_type == 4 && sr.reservation_id == id).Count(),
                                                single_nights = db.ReservationDetails.Where(sr => sr.room_type == 1 && sr.reservation_id == id).Select(sr => sr.no_of_days).Sum(),
                                                double_nights = db.ReservationDetails.Where(sr => sr.room_type == 2 && sr.reservation_id == id).Select(sr => sr.no_of_days).Sum(),
                                                triple_nights = db.ReservationDetails.Where(sr => sr.room_type == 3 && sr.reservation_id == id).Select(sr => sr.no_of_days).Sum(),
                                                quad_nights = db.ReservationDetails.Where(sr => sr.room_type == 4 && sr.reservation_id == id).Select(sr => sr.no_of_days).Sum(),
                                                //profit = calculateProfit(res.id).profit,
                                                ReservationDetailVM = (from resDet in db.ReservationDetails
                                                                       join client in db.Clients on resDet.client_id equals client.id
                                                                       select new ReservationDetailViewModel
                                                                       {
                                                                           res_id = resDet.id,
                                                                           reservation_from = resDet.reservation_from,
                                                                           reservation_to = resDet.reservation_to,
                                                                           room_type = resDet.room_type,
                                                                           client_first_name = client.first_name,
                                                                           client_last_name = client.last_name,

                                                                       }).Where(rd => rd.res_id == id).ToList()
                                            }).Where(r => r.id == id).FirstOrDefault();
            var report = new Rotativa.ViewAsPdf("Invoice", resData);
            return report;
        }
        public JsonResult saveCreditCard(ReservationViewModel reservationViewModel)
        {
            ReservationCreditCard reservationCreditCard = new ReservationCreditCard();
            reservationCreditCard.reservation_id = reservationViewModel.id;
            reservationCreditCard.credit_card_number = reservationViewModel.credit_card_number;
            reservationCreditCard.security_code = reservationViewModel.security_code;
            reservationCreditCard.card_expiration_date = reservationViewModel.card_expiration_date;
            db.ReservationCreditCards.Add(reservationCreditCard);
            db.SaveChanges();

            List<ReservationCreditCardViewModel> reservationCreditCards = db.ReservationCreditCards.Where(resCre => resCre.reservation_id == reservationViewModel.id).Select(resCre => new ReservationCreditCardViewModel
            {
                id = resCre.id,
                security_code = resCre.security_code,
                credit_card_number = resCre.credit_card_number,
                card_expiration_date = resCre.card_expiration_date
            }).ToList();

            return Json(new { reservationCreditCards = reservationCreditCards }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult addNight(ReservationDetailViewModel detailViewModel)
        {
            ReservationDetail detail = AutoMapper.Mapper.Map<ReservationDetailViewModel, ReservationDetail>(detailViewModel);
            Reservation reservation = db.Reservations.Find(detail.reservation_id);

            int Days = (detailViewModel.reservation_to - detailViewModel.reservation_from).Days;
            detail.no_of_days = Days + 1;
            var amount = (detailViewModel.room_price * Days);
            detail.tax = amount * (reservation.tax / 100);
            detail.amount = amount;
            detail.amount_after_tax = amount + detail.tax;
            detail.created_at = DateTime.Now;
            detail.created_by = Session["id"].ToString().ToInt();
            detail.active = 1;
            db.ReservationDetails.Add(detail);
            db.SaveChanges();

            detail.parent_id = detailViewModel.parent_id;
            db.SaveChanges();

            List<ReservationDetail> reservationDetails = db.ReservationDetails.Where(r => r.reservation_id == detailViewModel.reservation_id).ToList();
            if (reservationDetails.Count() != 0)
            {
                DateTime minDate = reservationDetails.Select(s => s.reservation_from).Min();
                DateTime maxDate = reservationDetails.Select(s => s.reservation_to).Max();

                reservation.check_in = minDate;
                reservation.check_out = maxDate;
                reservation.balance_due_date = Convert.ToDateTime(reservation.check_in).AddDays(-30);
            }

            ReservationService.UpdateTotals(reservation.id);

            detail.client_id = db.ReservationDetails.Find(detailViewModel.parent_id).client_id;
            db.SaveChanges();

            return Json(new { msg = "done" }, JsonRequestBehavior.AllowGet);
        }
    }
}