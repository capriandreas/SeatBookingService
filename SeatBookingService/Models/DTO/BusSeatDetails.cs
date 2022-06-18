using System.Collections.Generic;

namespace SeatBookingService.Models.DTO
{
    public class BusSeatDetails
    {
        public int trip_id { get; set; }
        public List<MSSeatDto> SeatsDetail { get; set; }
    }
}
