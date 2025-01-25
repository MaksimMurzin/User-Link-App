using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using KYC_APP.Models;
using KYC_APP.Controllers;
using KYC_APP.Services;
using Microsoft.AspNetCore.Mvc.Routing;

namespace KYC_APP_Test
{
    public class UserLinkControllerTests
    {
        private readonly Mock<IUserLinkService> _mockLinkService;
        private readonly UserLinkController _controller;
        private readonly Mock<IUrlHelper> _urlHelperMock;

        public UserLinkControllerTests()
        {
            _mockLinkService = new Mock<IUserLinkService>();
            _urlHelperMock = new Mock<IUrlHelper>();

            _controller = new UserLinkController(_mockLinkService.Object)
            {
                Url = _urlHelperMock.Object
            };
        }

        [Fact]
        public void Visit_ShouldReturnNoSecretsMessage_WhenLinkIsNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockLinkService.Setup(s => s.GetLink(id)).Returns((Link)null);

            // Act
            var result = _controller.Visit(id) as ContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("There are no secrets here.", result.Content);
        }

        [Fact]
        public void Visit_ShouldReturnNoSecretsMessage_WhenLinkIsExpired()
        {
            // Arrange
            var id = Guid.NewGuid();
            var link = new Link { ExpiryTime = DateTime.UtcNow.AddDays(-1) };
            _mockLinkService.Setup(s => s.GetLink(id)).Returns(link);

            // Act
            var result = _controller.Visit(id) as ContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("There are no secrets here.", result.Content);
        }

        [Fact]
        public void Visit_ShouldReturnNoSecretsMessage_WhenLinkIsUsed()
        {
            // Arrange
            var id = Guid.NewGuid();
            var link = new Link { Clicks = 10, MaxClicks = 2 };
            _mockLinkService.Setup(s => s.GetLink(id)).Returns(link);

            // Act
            var result = _controller.Visit(id) as ContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("There are no secrets here.", result.Content);
        }

        [Fact]
        public void Visit_ShouldReturnSecretMessageAndIncrementClicks_WhenLinkIsValid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var link = new Link { Username = "desiredUsername", Clicks = 0, ExpiryTime = DateTime.UtcNow.AddMinutes(10) };
            _mockLinkService.Setup(s => s.GetLink(id)).Returns(link);

            // Act
            var result = _controller.Visit(id) as ContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"You have found the secret, {link.Username}!", result.Content);
            Assert.Equal(1, link.Clicks);
            _mockLinkService.Verify(s => s.UpdateLink(link), Times.Once);
        }

        [Fact]
        public void Generate_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new LinkRequest
            {
                Username = "testuser",
                Expiry = TimeSpan.FromMinutes(10),
                MaxClicks = 5
            };
            var link = new Link
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                ExpiryTime = DateTime.UtcNow.Add(request.Expiry.Value),
                MaxClicks = request.MaxClicks
            };

            var httpContextMock = new Mock<HttpContext>();
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(r => r.Scheme).Returns("http");
            requestMock.Setup(r => r.Host).Returns(new HostString("localhost"));
            httpContextMock.Setup(h => h.Request).Returns(requestMock.Object);
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            _urlHelperMock
                .Setup(h => h.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://localhost/api/UserLink/Visit/" + link.Id);

            _mockLinkService
                .Setup(s => s.CreateLink(request.Username, request.Expiry, request.MaxClicks))
                .Returns(link);

            // Act
            var result = _controller.Generate(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;

            // Use reflection to assert the anonymous type's properties
            var urlProperty = value.GetType().GetProperty("Url");
            Assert.NotNull(urlProperty);
            var url = urlProperty.GetValue(value) as string;
            Assert.NotNull(url);
            Assert.Equal("http://localhost/api/UserLink/Visit/" + link.Id, url);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("user#")]
        [InlineData("thisusernameistoolongforthefiftycharacterlimitsetbytherules")]
        public void Generate_WithInvalidRequest_ReturnsBadRequest(string username)
        {
            // Arrange
            var request = new LinkRequest { Username = username };

            // Act
            var result = _controller.Generate(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Visit_WithValidId_ReturnsContentResult()
        {
            // Arrange
            var linkId = Guid.NewGuid();
            var link = new Link
            {
                Id = linkId,
                Username = "testuser",
                ExpiryTime = DateTime.UtcNow.AddMinutes(10),
                MaxClicks = 5,
                Clicks = 0
            };

            _mockLinkService
                .Setup(s => s.GetLink(linkId))
                .Returns(link);
            _mockLinkService
                .Setup(s => s.UpdateLink(It.IsAny<Link>()));

            // Act
            var result = _controller.Visit(linkId);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Contains("You have found the secret", contentResult.Content);
        }

        [Fact]
        public void Visit_WithInvalidId_ReturnsContentResult()
        {
            // Arrange
            var linkId = Guid.NewGuid();

            _mockLinkService
                .Setup(s => s.GetLink(linkId))
                .Returns((Link)null);

            // Act
            var result = _controller.Visit(linkId);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("There are no secrets here.", contentResult.Content);
        }

        [Fact]
        public void Visit_WithExpiredLink_ReturnsContentResult()
        {
            // Arrange
            var linkId = Guid.NewGuid();
            var link = new Link
            {
                Id = linkId,
                Username = "testuser",
                ExpiryTime = DateTime.UtcNow.AddMinutes(-10),
                MaxClicks = 5,
                Clicks = 0
            };

            _mockLinkService
                .Setup(s => s.GetLink(linkId))
                .Returns(link);

            // Act
            var result = _controller.Visit(linkId);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("There are no secrets here.", contentResult.Content);
        }

        [Fact]
        public void Visit_WithUsedLink_ReturnsContentResult()
        {
            // Arrange
            var linkId = Guid.NewGuid();
            var link = new Link
            {
                Id = linkId,
                Username = "testuser",
                ExpiryTime = DateTime.UtcNow.AddMinutes(-10),
                MaxClicks = 5,
                Clicks = 5
            };

            _mockLinkService
                .Setup(s => s.GetLink(linkId))
                .Returns(link);

            // Act
            var result = _controller.Visit(linkId);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("There are no secrets here.", contentResult.Content);
        }
    }
}
