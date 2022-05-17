using System;

namespace SeatBookingService.Models.DTO
{
    public class TRReservedSeatHeaderBookedDto
    {
        public int id { get; set; }
        public int users_id { get; set; }
        public decimal price { get; set; }
        public string additional_information { get; set; }
        public string no_bus { get; set; }
        public DateTime schedule_date { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string schedule_date_string { get; set; }
    }
}
