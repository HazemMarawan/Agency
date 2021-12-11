using Agency.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Agency.Models;
namespace Agency.Auth
{
    public class can
    {
        public static AgencyDbContext db = new AgencyDbContext();
        public static bool hasPermission(string permission)
        {
            return true;
            bool hasPermission = false;
            List<PermissionViewModel> permissionVM = HttpContext.Current.Session["user_permission"] as List<PermissionViewModel>;
            foreach (var permissionObj in permissionVM)
            {
                if (permission == permissionObj.name)
                {
                    hasPermission = true;
                    break;
                }
            }
            return hasPermission;
        }
        public static bool access()
        {
            if (HttpContext.Current.Session["user_name"] != null)
                return true;
            return false;
        }
    }
}