using KYC_APP.Models;
using Microsoft.Extensions.Caching.Memory;

namespace KYC_APP.Services
{
    public class UserLinkService : IUserLinkService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(10);

        public UserLinkService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public UserLink GenerateLink(string username, TimeSpan? expiry = null, int maxClicks = 1)
        {
            var link = new UserLink
            {
                Username = username,
                ExpiryTime = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : null,
                MaxClicks = maxClicks

            };

            _cache.Set(link.Id.ToString(), link, expiry ?? _defaultExpiry);
            return link;
        }

        public UserLink GetUserLink(Guid id)
        {
            _cache.TryGetValue(id.ToString(), out var link);
            return (UserLink)link; // need to improve this, may return nulls!!!
        }

        public void UpdateLink(UserLink link)
        {
            _cache.Set(link.Id.ToString(), link, link.ExpiryTime ?? DateTime.UtcNow.Add(_defaultExpiry));
        }
    }

}
