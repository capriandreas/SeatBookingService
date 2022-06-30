﻿using System;

namespace SeatBookingService.Models
{
    public class TRTripScheduleRoutes
    {
        public int id { get; set; }
        public string route { get; set; }
        public string city { get; set; }
        public int route_order { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }
    }
}
