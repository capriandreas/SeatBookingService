using System;

namespace SeatBookingService.Models.DTO
{
    public class TripDetailParamDto
    {
        public int route_id { get; set; }
        public int trip_type_id { get; set; }
        public DateTime schedule_date { get; set; }
        public string created_by { get; set; }
    }
}
