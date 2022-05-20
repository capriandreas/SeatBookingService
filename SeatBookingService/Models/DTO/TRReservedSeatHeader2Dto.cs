namespace SeatBookingService.Models.DTO
{
    public class TRReservedSeatHeader2Dto
    {
        public int seat_id { get; set; }
        public int reserved_seat_header_id { get; set; }
        public int trip_schedule_id { get; set; }
        public string seat_column { get; set; }
        public string seat_row { get; set; }
    }
}
