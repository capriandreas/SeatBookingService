namespace SeatBookingService.Models.DTO
{
    public class HistorySeatDetailDto
    {
        public int id { get; set; }
        public int seat_id { get; set; }
        public string seat_row { get; set; }
        public string seat_column { get; set; }
    }
}
