using Explorer.Payments.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class RedeemResultDto
    {
        public long TouristId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
