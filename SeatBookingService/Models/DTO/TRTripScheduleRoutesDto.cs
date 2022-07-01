using System;
using System.Collections.Generic;

namespace SeatBookingService.Models.DTO
{
    public class TRTripScheduleRoutesDto
    {
        public int id { get; set; }
        public int class_bus_id { get; set; }
        public DateTime? schedule_date { get; set; }
        public string departure_hours { get; set; }
        public string description { get; set; }
        public string created_by { get; set; }
        public List<TripScheduleRoutes> tripRoutes { get; set; }
    }

    public class TripScheduleRoutes
    {
        public string city { get; set; }
        public int route_order { get; set; }
    }
}
