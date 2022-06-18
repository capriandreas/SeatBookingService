using System.Collections.Generic;

namespace SeatBookingService.Models.DTO
{
    public class TRReservedSeatHeaderDto
    {
        public int users_id { get; set; }
        public int trip_id { get; set; }
        public decimal price { get; set; }
        public string additional_information { get; set; }
        public List<SeatDetail> seat_detail { get; set; }
    }

    public class SeatDetail
    {
        public int seat_id { get; set; }
    }
}
