namespace SeatBookingService.Models.DTO
{
    public class HistorySeatDetailDto
    {
        public int id { get; set; }
        public int seat_id { get; set; }
        public string seat_row { get; set; }
        public string seat_column { get; set; }
        public int status_seat_id { get; set; }
        public string status_name { get; set; }
        public int action_id { get; set; }
        public string action_name { get; set; }
        public bool allow_cancel { get; set; }
    }
}
