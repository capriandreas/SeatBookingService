﻿namespace SeatBookingService.Models.DTO
{
    public class MSTripDto
    {
        public int id_route { get; set; }
        public string Route { get; set; }
        public string departure_hours { get; set; }
        public string class_bus { get; set; }
        public string description { get; set; }
        public string trip_type { get; set; }
    }
}
