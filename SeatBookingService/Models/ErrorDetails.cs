using Newtonsoft.Json;

namespace SeatBookingService.Models
{
    public class ErrorDetails
    {
        public string datetimenow { get; set; }
        public string username { get; set; }
        public string path { get; set; }
        public string method { get; set; }
        public int status_code { get; set; }
        public string message { get; set; }
        public string stack_trace { get; set; }
    }
}
