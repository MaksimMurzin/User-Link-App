using KYC_APP.Models;
using KYC_APP.Services;
using Microsoft.AspNetCore.Mvc;

namespace KYC_APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLinkController : ControllerBase
    {
        private readonly IUserLinkService _linkService;

        public UserLinkController(IUserLinkService linkService)
        {
            _linkService = linkService;
        }

        [HttpPost]
        public IActionResult Generate([FromBody] LinkRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || request.Username.Length > 50 || !request.Username.All(char.IsLetterOrDigit))
            {
                return BadRequest("Invalid username.");
            }

            var link = _linkService.CreateLink(request.Username, request.Expiry, request.MaxClicks);
            return Ok(new { Url = Url.Action("Visit", "UserLink", new { id = link.Id }, Request.Scheme) });
        }

        [HttpGet("{id:guid}")]
        public IActionResult Visit(Guid id)
        {
            var link = _linkService.GetLink(id);

            if (link == null || link.IsExpired || link.IsUsed)
            {
                return Content("There are no secrets here.");
            }

            link.Clicks++;
            _linkService.UpdateLink(link);

            return Content($"You have found the secret, {link.Username}!");
        }
    }

}
