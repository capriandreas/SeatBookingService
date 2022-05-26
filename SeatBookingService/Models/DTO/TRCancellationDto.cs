namespace SeatBookingService.Models.DTO
{
    public class TRCancellationDto
    {
        public int reserved_seat_id { get; set; }
        public int status_seat_id { get; set; }
        public string status_name { get; set; }
        public int action_id { get; set; }
        public string action_name { get; set; }
        public string reason { get; set; }
    }
}
