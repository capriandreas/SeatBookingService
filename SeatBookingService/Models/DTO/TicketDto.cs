using System;
using System.Collections.Generic;

namespace SeatBookingService.Models.DTO
{
    public class TicketDto
    {
        public DateTime schedule_date { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string no_bus { get; set; }
        public decimal price { get; set; }
        public decimal total_price { get; set; }
        public List<TicketSeatDetail> ticket_seat_detail { get; set; }
    }

    public class TicketSeatDetail
    {
        public int seat_id { get; set; }
        public string seat_column { get; set; }
        public string seat_row { get; set; }
    }
}
