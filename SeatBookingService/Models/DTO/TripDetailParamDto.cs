using System;

namespace SeatBookingService.Models.DTO
{
    public class TripDetailParamDto
    {
        public int id_route { get; set; }
        public int trip_type_id { get; set; }
        public DateTime schedule_date { get; set; }
    }
}
