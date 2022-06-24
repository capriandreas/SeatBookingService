namespace SeatBookingService.Models.DTO
{
    public class ChangePasswordDto
    {
        public int user_id { get; set; }
        public string password { get; set; }
        public string verify_password { get; set; }
        public string encrypted_password { get; set; }
    }
}
