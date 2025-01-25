using KYC_APP.Controllers;
using KYC_APP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KYC_APP_Test
{
    public class AdminControllerTests
    {
        private readonly Mock<IUserLinkService> _mockLinkService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockLinkService = new Mock<IUserLinkService>();
            _controller = new AdminController(_mockLinkService.Object);

            // Setup for the ControllerContext to mock HTTP context for Url.Action
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };

            // Setting up the UrlHelper
            var urlHelperMock = new Mock<IUrlHelper>();
        }

        [Fact]
        public void Index_ShouldReturnView()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("username_with_special_chars!")]
        [InlineData("username_that_is_way_too_long_for_the_system_to_handle_correctly_so_it_should_fail")]
        public void Generate_ShouldReturnBadRequest_WhenUsernameIsInvalid(string username)
        {
            // Act
            var result = _controller.Generate(username) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Invalid username.", result.Value);
        }
    }
}
