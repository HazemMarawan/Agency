using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Agency.Models;
namespace Agency.Helpers
{
    public static class Logs
    {
        public static AgencyDbContext db = new AgencyDbContext();
        public static void ReservationActionLog(int? user_id,int? reservation_id,string action,string description)
        {
            ReservationLog log = new ReservationLog();
            log.user_id = user_id;
            log.reservation_id = reservation_id;
            log.action = action;
            log.description = description;
            log.created_at = DateTime.Now;

            db.ReservationLogs.Add(log);
            db.SaveChanges();
        }
    }
}