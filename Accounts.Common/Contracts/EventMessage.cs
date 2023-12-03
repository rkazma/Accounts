namespace Common.Contracts
{
    public class EventMessage
    {
        public long CustomerId { get; set; }
        public long AccountId { get; set; }
        public decimal InitialCredit { get; set; }
        public int ResultCode { get; set; }
        public string EndPoint { get; set; }
    }
}
