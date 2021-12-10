using Agency.Auth;
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
    public class CommentController : Controller
    {
        AgencyDbContext db = new AgencyDbContext();

        // GET: Comment
        public ActionResult Index()
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
                                     //join res in db.Reservations on comm.reservation_id equals res.id
                                     select new ReservationCommentViewModel
                                     {
                                         id = comm.id,
                                         comment = comm.comment,
                                         created_by_string = comment_by.full_name,
                                         created_at_string = comm.created_at.ToString(),
                                         reservation_id = comm.reservation_id
                                     });

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

            return View();
        }
    }
}