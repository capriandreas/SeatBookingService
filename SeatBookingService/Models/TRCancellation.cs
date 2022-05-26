using System;

namespace SeatBookingService.Models
{
    public class TRCancellation
    {
        public int id { get; set; }
        public int reserved_seat_id { get; set; }
        public int status_seat_id { get; set; }
        public string reason { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
        public string kelas_bus { get; set; }
    }
}
