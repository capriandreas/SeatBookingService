namespace SeatBookingService.Models
{
    public class BusinessLogicResult
    {
        public bool result { get; set; }
        public string message { get; set; }
    }
    public class BusinessLogicResult<T> : BusinessLogicResult
    {
        public T data { get; set; }
    }
}
