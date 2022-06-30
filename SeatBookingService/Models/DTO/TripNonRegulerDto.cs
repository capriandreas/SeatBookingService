using System;

namespace SeatBookingService.Models.DTO
{
    public class TripNonRegulerDto
    {
        public int id_route { get; set; }
        public DateTime? schedule_date { get; set; }
        public string Route { get; set; }
        public string departure_hours { get; set; }
        public string class_bus { get; set; }
        public string description { get; set; }
    }
}
