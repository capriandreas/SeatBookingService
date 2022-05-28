using System;

namespace SeatBookingService.Models.DTO
{
    public class HistoryHeaderDto
    {
        public int trip_schedule_id { get; set; }
        public int users_id { get; set; }
        public DateTime schedule_date { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string no_bus { get; set; }
    }
}
