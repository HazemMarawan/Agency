using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.Enums
{
    public enum UserType
    {
        Male = 1,
        Female = 2,
    }
    public enum Gender
    {
        Male = 1,
        Female = 2,
    }
    public enum HotelFacilities
    {
        SwimmingPool = 1,
        Wifi = 2,
        Gym = 3,
        AirConditioning = 4
    }
    public enum HotelBenefits
    {
        Shuttle = 1,
        Breakfast = 2
    }
    public enum Currency
    {
        USD = 1,
        GBP = 2,
        EUR = 3,
    }
    public enum PaymentStatus
    {
        Partially = 1,
        Paid = 2
    }
    public enum Shift
    {
        International = 1,
        USA = 2,
        Private = 3
    }
    public enum TaskStatus
    {
        Pending = 0,
        Active = 1,
        Done = 2
    }
}