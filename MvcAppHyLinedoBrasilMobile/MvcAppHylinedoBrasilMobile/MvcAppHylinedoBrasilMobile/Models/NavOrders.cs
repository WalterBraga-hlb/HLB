using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAppHylinedoBrasilMobile.Models
{
    public class NavOrders
    {
        public int ID { get; set; }

        public string Location { get; set; }
        public string LocationColor { get; set; }
        public string CustomerName { get; set; }
        public string Country { get; set; }
        public int NumeroSemanaAno { get; set; }
        public int NumeroSemanaMes { get; set; }
        public DateTime HatchDate { get; set; }
        public DateTime SetDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string OrderNo { get; set; }
        public string NavOrderNo { get; set; }
        public string Line { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
    }
}