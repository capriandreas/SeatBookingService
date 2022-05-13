using System;

namespace SeatBookingService.Models.DTO
{
    public class TRTripScheduleDto
    {
        public int id { get; set; }
        public DateTime? schedule_date { get; set; }
        public string origin { get; set; }
        public string origin_additional_information { get; set; }
        public string destination { get; set; }
        public string destination_additional_information { get; set; }
        public string no_bus { get; set; }
        public string created_by { get; set; }
        public int jumlah_seat { get; set; }
        public string no_polisi { get; set; }
        public int kelas_id { get; set; }
        public string kelas_bus { get; set; }
    }
}
