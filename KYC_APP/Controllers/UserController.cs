using KYC_APP.Models;
using KYC_APP.Services;
using Microsoft.AspNetCore.Mvc;

namespace KYC_APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserLinkService _service;

        public UserController(IUserLinkService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Generate([FromBody] LinkRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || request.Username.Length > 50 || !request.Username.All(char.IsLetterOrDigit))
            {
                return BadRequest("Invalid username.");
            }

            var link = _service.GenerateLink(request.Username, request.Expiry, request.MaxClicks);
            return Ok(new { Url = Url.Action("Visit", "Link", new { id = link.Id }, Request.Scheme) });
        }

        [HttpGet("{id:guid}")]
        public IActionResult Visit(Guid id)
        {
            var link = _service.GetUserLink(id);

            if (link == null || link.IsExpired || link.IsClicked)
            {
                return Content("There are no secrets here.");
            }

            link.Clicks++;
            _service.UpdateLink(link);

            return Content($"You have found the secret, {link.Username}!");
        }
    }

}
