using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Services;
using MRA.Web.Utils;
using System.Security.Claims;
using SixLabors.ImageSharp;
using MRA.Web.Models.Art;
using MRA.Web.Models.Admin;
using MRA.Services.Firebase.Models;
using System.IO;
using SixLabors.ImageSharp.Processing;
using System;
using Azure.Storage.Blobs.Models;

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
                    var userId = SessionSettings.USER_ID_ADMIN;
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
        [AutorizacionRequerida]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.SetString(SessionSettings.USER_ID, "");
            HttpContext.Session.SetString(SessionSettings.USER_NAME, "");
            return RedirectToAction("Index", "Home");
        }



        [AutorizacionRequerida]
        public async Task<IActionResult> EditDrawing(string id)
        {
            var drawing = await _drawingService.FindDrawingById(id, false);
            return View(drawing);
        }

        [HttpPost]
        [AutorizacionRequerida]
        public async Task<Drawing> SaveDrawing(Drawing drawing)
        {
            try
            {
                var userId = HttpContext.Session.GetString(SessionSettings.USER_ID) ?? "";
                if (!SessionSettings.IsLogedAsAdmin(userId))
                {
                    ViewBag.Error = "No eres administrador y no tienes permiso para editarlo";
                    return null;
                }

                Drawing result = null;
                if (!String.IsNullOrEmpty(drawing.Id))
                {
                    //result = await _drawingService.AddAsync(drawing);
                    return drawing;
                }

                if(result == null)
                {
                    ViewBag.Error = "Ha ocurrido un error por el que no se ha guardado el dibujo";
                    return null;
                }

                return result;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ha ocurrido un fallo al calcular la nota final.";
                return null;
            }
        }

        [HttpPost]
        [AutorizacionRequerida]
        public async Task<bool> CheckAzurePath(string id)
        {
            return await _drawingService.ExistsBlob(id);
        }

        [HttpPost]
        [AutorizacionRequerida]
        public async Task<IActionResult> UploadAzureImage(int azureImageThumbnailSize, string path, IFormFile azureImage)
        {
            try
            {
                if (azureImage == null || azureImage.Length == 0)
                {
                    return new JsonResult(new
                    {
                        error = "No se ha proporcionado ninguna imagen",
                    });
                }

                var blobLocationThumbnail = _drawingService.CrearThumbnailName(path);
                await UploadImage(azureImage, blobLocationThumbnail, azureImageThumbnailSize);
                await UploadImage(azureImage, path, 0);

                var urlBase = _drawingService.GetAzureUrlBase();
                var url = urlBase + path;
                var url_tn = urlBase + blobLocationThumbnail;


                return new JsonResult(new
                {
                    error = "",
                    url = url,
                    url_tn = url_tn,
                    tn = blobLocationThumbnail
                });
            }catch(Exception ex)
            {
                return new JsonResult(new
                {
                    error = ex.Message,
                });
            }
        }

        private async Task UploadImage(IFormFile azureImage, string path, int azureImageThumbnailSize)
        {
            using (var imageStream = new MemoryStream())
            {
                await azureImage.CopyToAsync(imageStream);
                imageStream.Position = 0;

                await _drawingService.RedimensionarYGuardarEnAzureStorage(imageStream, path, azureImageThumbnailSize);
            }
        }
    }
}
