using System.Collections.Generic;

namespace SeatBookingService.Models.DTO
{
    public class TRStationRoutesDto
    {
        public int class_bus_id { get; set; }
        public string departure_hours { get; set; }
        public string description { get; set; }
        public string created_by { get; set; }
        public List<MSStationRoutes> stationRoutes { get; set; }
    }

    public class MSStationRoutes
    {
        public string city { get; set; }
        public int route_order { get; set; }
    }
}
