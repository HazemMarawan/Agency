using Agency.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.Auth
{
    public class can
    {
        public static bool access()
        {
            if (HttpContext.Current.Session["user_name"] != null)
                return true;
            return false;
        }
    }
}