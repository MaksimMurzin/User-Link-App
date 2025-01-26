using KYC_APP.Services;
using Microsoft.AspNetCore.Mvc;

namespace KYC_APP.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserLinkService _linkService;

        public AdminController(IUserLinkService linkService)
        {
            _linkService = linkService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generate(string username)
        {
            if (string.IsNullOrEmpty(username) || username.Length > 50 || !username.All(char.IsLetterOrDigit))
            {
                return BadRequest("Invalid username.");
            }

            var link = _linkService.CreateLink(username);

            // for some weird reason model was not working so I used viewbag
            ViewBag.Username = username;
            ViewBag.GeneratedLink = Url.Action("Visit", "UserLink", new { id = link.Id }, Request.Scheme);
            return View("Generated");
        }
    }
}
