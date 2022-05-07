using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SeatBookingService.Models
{
    public class APIResult
    {
        public HttpStatusCode httpCode { get; set; }
        public bool is_ok { get; set; }
        public string message { get; set; }
    }

    public class APIResult<T> : APIResult
    {
        public T data { get; set; }
        public int data_records { get; set; }
    }
}
