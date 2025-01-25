using KYC_APP.Models;
using Microsoft.Extensions.Caching.Memory;

namespace KYC_APP.Services
{
    public class UserLinkService : IUserLinkService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(10);
        private readonly int _defaultMaxClicks = 1;

        public UserLinkService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Link CreateLink(string username)
        {
            var link = new Link
            {
                Username = username,
                ExpiryTime = DateTime.UtcNow.Add(_defaultExpiry),
                MaxClicks = _defaultMaxClicks
            };
            _cache.Set(link.Id.ToString(), link, _defaultExpiry);
            return link;
        }

        public Link CreateLink(string username, TimeSpan? expiry = null, int maxClicks = 1)
        {
            var link = new Link
            {
                Username = username,
                ExpiryTime = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : null,
                MaxClicks = maxClicks
            };
            _cache.Set(link.Id.ToString(), link, expiry ?? _defaultExpiry);
            return link;
        }

        public Link GetLink(Guid id)
        {
            _cache.TryGetValue(id.ToString(), out Link link);
            return link;
        }

        public void UpdateLink(Link link)
        {
            _cache.Set(link.Id.ToString(), link, link.ExpiryTime ?? DateTime.UtcNow.Add(_defaultExpiry));
        }
    }
}
