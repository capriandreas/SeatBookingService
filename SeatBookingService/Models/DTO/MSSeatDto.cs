namespace SeatBookingService.Models.DTO
{
    public class MSSeatDto
    {
        public int seat_id { get; set; }
        public string no_bus { get; set; }
        public string seat_column { get; set; }
        public string seat_row { get; set; }
        public int? booked_by { get; set; }
        public string seat_status { get; set; }
        public int? trip_schedule_id { get; set; }
    }
}
