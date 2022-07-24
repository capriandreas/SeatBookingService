namespace SeatBookingService.Models.DTO
{
    public class MSSeatDto
    {
        public int? reserved_seat_id { get; set; }
        public int seat_id { get; set; }
        public int? class_bus_id { get; set; }
        public string seat_column { get; set; }
        public string seat_row { get; set; }
        public int? users_id { get; set; }
        public string nickname { get; set; }
        public int status_seat_id { get; set; }
        public string seat_status { get; set; }
        public string status_name { get; set; }
        public string reason { get; set; }
        public int trip_id { get; set; }
        public int trip_type_id { get; set; }
        public int route_id { get; set; }
    }
}
