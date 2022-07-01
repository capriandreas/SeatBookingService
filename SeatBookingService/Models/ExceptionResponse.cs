using Newtonsoft.Json;

namespace SeatBookingService.Models
{
    public class ExceptionResponse
    {
        public int status_code { get; set; }
        public string message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
