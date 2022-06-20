using System;

namespace SeatBookingService.Models.DTO
{
    public class TRCancellationDto
    {
        public int id { get; set; }
        public int reserved_seat_id { get; set; }
        public int status_seat_id { get; set; }
        public string status_name { get; set; }
        public int action_id { get; set; }
        public string action_name { get; set; }
        public string reason { get; set; }
        public int trip_id { get; set; }
        public int route_id { get; set; }
        public int trip_type_id { get; set; }
        public DateTime schedule_date { get; set; }
        public string seat_column { get; set; }
        public string seat_row { get; set; }
        public string nickname { get; set; }
        public string route { get; set; }
        public string departure_hours { get; set; }
        public string cancel_by_users_id { get; set; }
        public string cancel_by_users_nickname { get; set; }
    }
}
