using System.Collections.Generic;

namespace Explorer.Payments.API.Dtos
{
    public class CheckoutRequestDto
    {
        
        public Dictionary<int, string> AffiliateCodesByTourId { get; set; } = new();
    }
}
