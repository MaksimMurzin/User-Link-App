using KYC_APP.Models;

namespace KYC_APP.Services
{
    public interface IUserLinkService
    {
        Link CreateLink(string username, TimeSpan? expiry = null, int maxClicks = 1);
        Link CreateLink(string username);
        Link GetLink(Guid id);
        void UpdateLink(Link link);
    }
}