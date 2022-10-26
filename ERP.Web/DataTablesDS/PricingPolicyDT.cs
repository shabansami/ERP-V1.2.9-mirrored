using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class PricingPolicyDT
    {
        public Guid Id { get; set; }
        public Guid? PricingPolicyId { get; set; }
        public string PricingPolicyName { get; set; }
        public double? SellPricePolicy { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Actions { get; set; }
        public string Num { get; set; } 
    }
}