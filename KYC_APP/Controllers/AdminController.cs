using KYC_APP.Services;
using Microsoft.AspNetCore.Mvc;

namespace KYC_APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IUserLinkService _service;

        public AdminController(IUserLinkService service)
        {
            _service = service;
        }

        [HttpPost("generate")]
        public IActionResult GenerateLink([FromBody] string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length > 50 || !username.All(char.IsLetterOrDigit))
            {
                return BadRequest("Invalid username.");
            }

            var link = _service.GenerateLink(username);
            var url = Url.Action("Visit", "Link", new { id = link.Id }, Request.Scheme);
            return Ok(url);
        }

        [HttpPost]
        public IActionResult Generate(string username)
        {
            if (string.IsNullOrEmpty(username) || username.Length > 50 || !username.All(char.IsLetterOrDigit))
            {
                return BadRequest("Invalid username.");
            }

            var link = _service.GenerateLink(username);
            var url = Url.Action("Visit", "Link", new { id = link.Id }, Request.Scheme);
            return View("Generated", url);
        }

        // sane check, needs to be deleted
        [HttpGet]
        public IActionResult Get() 
        {
            return Ok("Everything is sane");
        }
    }

}
