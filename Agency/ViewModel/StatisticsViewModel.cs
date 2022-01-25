using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.ViewModel
{
    public class StatisticsViewModel
    {
        public double? collected { get; set; }
        public double? collected_percentage { get; set; }
        public double? balance { get; set; }
        public double? balance_percentage { get; set; }
        public double? profit { get; set; }
        public double? profit_percentage { get; set; }
        public double? refund { get; set; }
        public double? refund_percentage { get; set; }
        public double? total_amount_after_tax_sum { get; set; }
        public double? total_amount_after_tax_sum_for_profit { get; set; }
        public int? total_nights { get; set; }
    }
}