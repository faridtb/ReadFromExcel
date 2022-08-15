using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmptyAPI.Data.Entities
{
    public class ExcelData
    {
        public int Id { get; set; }
        public string Segment { get; set; }
        public string Country { get; set; }
        public string Product { get; set; }
        public string DiscountBand { get; set; }
        public double UnitsSold { get; set; }
        public double ManufacturingPrice { get; set; }
        public double SellPrice { get; set; }
        public double GrossSales { get; set; }
        public double Discount { get; set; }
        public double Sale { get; set; }
        public double COGS { get; set; }
        public double Profit { get; set; }
        public DateTime Date { get; set; }
    }
}
