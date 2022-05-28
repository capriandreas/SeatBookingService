﻿using System;

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
        public string no_bus { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public DateTime schedule_date { get; set; }
        public string seat_column { get; set; }
        public string seat_row { get; set; }
    }
}