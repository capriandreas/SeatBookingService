namespace SeatBookingService.Models.DTO
{
    public class GetSummaryReportDto
    {
        public string no_bus { get; set; }
        public string no_polisi { get; set; }
        public string class_bus { get; set; }
        public string status_bus { get; set; }
        public string station { get; set; }
        public string description { get; set; }
        public int trip_type_id { get; set; }
        public int route_id { get; set; }
        public string route { get; set; }
    }
}
