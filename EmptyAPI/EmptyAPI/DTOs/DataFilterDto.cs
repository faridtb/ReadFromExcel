using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmptyAPI.DTOs
{
    public class DataFilterDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AcceptorEmail { get; set; }

    }

    public enum Filter
    {
        Segment = 1,
        Country,
        Product,
        Discount
    }
}
