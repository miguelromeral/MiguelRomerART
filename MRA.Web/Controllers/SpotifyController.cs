using Microsoft.AspNetCore.Mvc;

namespace MRA.Web.Controllers
{
    public class SpotifyController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Playlist");
        }

        public IActionResult Playlist()
        {
            return View();
        }
    }
}
