using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace paypalAPI
{
    public class PayPalOrder
    {
        public string OrderId { get; set; }
    }
    public class PayPalCapture
    {
        public string OrderId { get; set; }
        public string CaptureId { get; set; }
    }
}
