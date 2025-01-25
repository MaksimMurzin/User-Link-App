using KYC_APP.Models;
using KYC_APP.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace KYC_APP_Test
{
    public class UserLinkServiceTests
    {
        private readonly UserLinkService _service;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly Mock<ICacheEntry> _cacheEntryMock;

        public UserLinkServiceTests()
        {
            _cacheMock = new Mock<IMemoryCache>();
            _cacheEntryMock = new Mock<ICacheEntry>();
            _service = new UserLinkService(_cacheMock.Object);
        }

        [Fact]
        public void CreateLink_WithUsername_ReturnsLink()
        {
            var username = "test_user";
            object cacheValue = null;

            _cacheMock
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheValue))
                .Returns(false);

            _cacheMock
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(_cacheEntryMock.Object);

            var result = _service.CreateLink(username);

            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
            Assert.Equal(1, result.MaxClicks);
            _cacheMock.Verify(m => m.CreateEntry(result.Id.ToString()), Times.Once);
        }

        [Fact]
        public void CreateLink_WithExpiryAndMaxClicks_ReturnsLink()
        {
            var username = "test_user";
            var expiry = TimeSpan.FromMinutes(5);
            var maxClicks = 5;
            object cacheValue = null;

            _cacheMock
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out cacheValue))
                .Returns(false);

            _cacheMock
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(_cacheEntryMock.Object);

            var result = _service.CreateLink(username, expiry, maxClicks);

            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
            Assert.Equal(maxClicks, result.MaxClicks);
            _cacheMock.Verify(m => m.CreateEntry(result.Id.ToString()), Times.Once);
        }

        [Fact]
        public void GetLink_WithValidId_ReturnsLink()
        {
            var link = new Link
            {
                Username = "test_user",
                ExpiryTime = DateTime.UtcNow.AddMinutes(10),
                MaxClicks = 1
            };

            _cacheMock
                .Setup(m => m.TryGetValue(link.Id.ToString(), out It.Ref<object>.IsAny))
                .Returns((string key, out object value) =>
                {
                    value = link;
                    return true;
                });

            var result = _service.GetLink(link.Id);

            Assert.NotNull(result);
            Assert.Equal(link.Username, result.Username);
        }

        [Fact]
        public void UpdateLink_WithValidLink_UpdatesCache()
        {
            var link = new Link
            {
                Username = "test_user",
                ExpiryTime = DateTime.UtcNow.AddMinutes(10),
                MaxClicks = 1
            };

            _cacheMock
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(_cacheEntryMock.Object);

            _service.UpdateLink(link);

            _cacheMock.Verify(m => m.CreateEntry(link.Id.ToString()), Times.Once);
        }
    }
}
