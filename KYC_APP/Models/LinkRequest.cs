namespace KYC_APP.Models
{
    public class LinkRequest
    {
        public string Username { get; set; }
        public TimeSpan? Expiry { get; set; }
        public int MaxClicks { get; set; } = 1;
    }
}
