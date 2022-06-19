namespace SeatBookingService.Models.DTO
{
    public class TRExpeditionDto
    {
        public int id { get; set; }
        public int users_id { get; set; }
        public int trip_id { get; set; }
        public decimal price { get; set; }
        public string goods_type { get; set; }
        public string volume { get; set; }
        public string additional_information { get; set; }
    }
}
