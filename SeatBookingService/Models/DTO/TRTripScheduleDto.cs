using System;

namespace SeatBookingService.Models.DTO
{
    public class TRTripScheduleDto
    {
        public DateTime? schedule_date { get; set; }
        public string origin { get; set; }
        public string origin_additional_information { get; set; }
        public string destination { get; set; }
        public string destination_additional_information { get; set; }
        public string no_bus { get; set; }
        public string created_by { get; set; }
    }
}
