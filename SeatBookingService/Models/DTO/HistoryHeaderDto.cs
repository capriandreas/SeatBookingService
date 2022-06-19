using System;

namespace SeatBookingService.Models.DTO
{
    public class HistoryHeaderDto
    {
        public int trip_id { get; set; }
        public DateTime schedule_date { get; set; }
        public int users_id { get; set; }
        public decimal price { get; set; }
        public int total_tickets { get; set; }
        public decimal total_price { get; set; }
        public string additional_information { get; set; }
        public string route { get; set; }
    }
}
