using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmptyAPI.DTOs
{
    public class DataReturnDto
    {
        public string Type { get; set; }
        public int TotalCount { get; set; }
        public double Sale { get; set; }
        public double Discount { get; set; }
        public double Profit { get; set; }
    }
}
