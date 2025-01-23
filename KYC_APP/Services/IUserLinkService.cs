using KYC_APP.Models;

namespace KYC_APP.Services
{
    public interface IUserLinkService
    {
        UserLink GenerateLink(string username, TimeSpan? expiry = null, int maxClicks = 1);
        UserLink GetUserLink(Guid id);
        void UpdateLink(UserLink link);
    }
}