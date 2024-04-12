using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Services;
using MRA.Web.Utils;
using System.Security.Claims;
using SixLabors.ImageSharp;

namespace MRA.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly DrawingService _drawingService;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public AdminController(ILogger<HomeController> logger, DrawingService drawingService, IConfiguration configuration)
        {
            _logger = logger;
            _drawingService = drawingService;
            _configuration = configuration;
        }


        [AutorizacionRequerida]
        public IActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostLogin(string password)
        {
            var error = "";
            if (String.IsNullOrEmpty(password))
            {
                ViewBag.Error = "La contraseña es obligatoria";
                return View();
            }

            try
            {
                var passwordAppSettings = _configuration["Administrator:Password"];

                if (passwordAppSettings.Equals(password))
                {
                    var userId = "Admin";
                    var userName = "MiguelRomeral";

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userId),
                        new Claim(ClaimTypes.Role, "Administrator")
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    await PerformLogin(
                        userId: userId,
                        userName: userName
                        );

                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    error = "Contraseña incorrecta";
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return RedirectToAction("Login", new { error = error });
        }

        private async Task PerformLogin(string userId, string userName)
        {
            if (String.IsNullOrEmpty(userId))
            {
                throw new Exception("User ID cannot be empty");
            }

            HttpContext.Session.SetString(SessionSettings.USER_ID, userId);
            HttpContext.Session.SetString(SessionSettings.USER_NAME, userName);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.SetString(SessionSettings.USER_ID, "");
            HttpContext.Session.SetString(SessionSettings.USER_NAME, "");
            return RedirectToAction("Index", "Home");
        }
    }
}
