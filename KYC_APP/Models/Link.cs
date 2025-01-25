namespace KYC_APP.Models
{
    public class Link
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public DateTime? ExpiryTime { get; set; }
        public int MaxClicks { get; set; } = 1;
        public int Clicks { get; set; } = 0;

        public bool IsExpired => ExpiryTime.HasValue && DateTime.UtcNow > ExpiryTime;
        public bool IsUsed => Clicks >= MaxClicks;
    }
}
