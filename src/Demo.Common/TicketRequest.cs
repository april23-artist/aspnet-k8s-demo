namespace Demo.Common
{
    public class TicketRequest
    {
        public string UserId { get; set; } = default!;
        public string EventId { get; set; } = default!;
        public int Quantity { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
