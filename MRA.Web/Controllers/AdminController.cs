using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Web.Utils;
using System.Security.Claims;
using SixLabors.ImageSharp;
using MRA.Web.Models.Art;
using MRA.Web.Models.Admin;
using MRA.DTO.Firebase.Models;
using System.IO;
using SixLabors.ImageSharp.Processing;
using System;
using Azure.Storage.Blobs.Models;
using MRA.Web.Models;
using System.Diagnostics;
using System.Collections.Generic;
using MRA.Services;
using Microsoft.Extensions.Options;
using MRA.DTO.Options;

namespace MRA.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly DrawingService _drawingService;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AdministratorOptions _administratorOptions;

        public AdminController(ILogger<HomeController> logger, DrawingService drawingService, IConfiguration configuration, IOptions<AdministratorOptions> adminOptions)
        {
            _logger = logger;
            _drawingService = drawingService;
            _configuration = configuration;
            _administratorOptions = adminOptions.Value;
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
                var passwordAppSettings = _administratorOptions.Password;

                if (passwordAppSettings.Equals(password))
                {
                    var userId = SessionSettings.USER_ID_ADMIN;
                    var userName = _administratorOptions.User;

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
        public async Task<IActionResult> NewDrawing(string id)
        {
            var model = new EditDrawingViewModel();
            model.Drawing = new Drawing();
            return View("EditDrawing", model);
        }

        [AutorizacionRequerida]
        public async Task<IActionResult> EditDrawing(string id)
        {
            var model = new EditDrawingViewModel();
            model.Drawing = await _drawingService.FindDrawingById(id, false, false, false);
            if(model.Drawing == null)
            {
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            return View("EditDrawing", model);
        }

        [HttpPost]
        [AutorizacionRequerida]
        public async Task<Drawing> SaveDrawing([FromForm] EditDrawingViewModel model)
        {
            try
            {
                var drawing = model.Drawing;
                var userId = HttpContext.Session.GetString(SessionSettings.USER_ID) ?? "";
                if (!SessionSettings.IsLogedAsAdmin(userId))
                {
                    ViewBag.Error = "No eres administrador y no tienes permiso para editarlo";
                    return null;
                }

                drawing.Name = drawing.Name ?? "";
                drawing.ModelName = drawing.ModelName ?? "";
                drawing.ProductName = drawing.ProductName ?? "";
                drawing.ReferenceUrl = drawing.ReferenceUrl ?? "";
                drawing.Path = drawing.Path ?? "";
                drawing.PathThumbnail = _drawingService.CrearThumbnailName(drawing.Path);
                drawing.Title = drawing.Title ?? "";
                drawing.SpotifyUrl = drawing.SpotifyUrl ?? "";
                drawing.Tags = (drawing.TagsText ?? "").Split(Drawing.SEPARATOR_TAGS).ToList();

                Drawing result = null;
                if (!String.IsNullOrEmpty(drawing.Id))
                {
                    result = await _drawingService.AddAsync(drawing);
                    return drawing;
                }

                if(result == null)
                {
                    ViewBag.Error = "Ha ocurrido un error por el que no se ha guardado el dibujo";
                    return null;
                }

                _drawingService.CleanAllCache();
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
        public async Task<JsonResult> CheckAzurePath(string id)
        {
            var existe = await _drawingService.ExistsBlob(id);

            var blobLocationThumbnail = _drawingService.CrearThumbnailName(id);

            var urlBase = _drawingService.GetAzureUrlBase();
            var url = urlBase + id;
            var url_tn = urlBase + blobLocationThumbnail;

            return new JsonResult(new 
            {
                existe,
                url,
                url_tn
            });
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

        [HttpPost]
        [AutorizacionRequerida]
        public async Task<bool> ExisteDrawingId(string id)
        {
            try
            {
                var drawing = await _drawingService.FindDrawingById(id, false);
                return drawing != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        [AutorizacionRequerida]
        public async Task<IActionResult> ListCollections()
        {
            var model = new ListCollectionsViewModel();
            var drawings = await _drawingService.GetAllDrawings();
            model.Collections = await _drawingService.GetAllCollections(drawings, false);
            return View(model);
        }

        [AutorizacionRequerida]
        public async Task<IActionResult> EditCollection(string id)
        {
            var list = await _drawingService.GetAllDrawings();
            var col = await _drawingService.FindCollectionById(id, list, false);
            var model = new EditCollectionsViewModel()
            {
                ListDrawings = list,
                Collection = col,
                DrawingsId = String.Join(";", col.Drawings.Select(x => x.Id))
            };  

            return View(model);
        }


        [HttpPost]
        [AutorizacionRequerida]
        public async Task<bool> ExisteCollectionId(string id)
        {
            try
            {
                var drawings = await _drawingService.GetAllDrawings();
                var collection = await _drawingService.FindCollectionById(id, drawings);
                return collection != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        [AutorizacionRequerida]
        public async Task<Collection> SaveCollection([FromForm] IFormCollection model)
        {
            try
            {
                var userId = HttpContext.Session.GetString(SessionSettings.USER_ID) ?? "";
                if (!SessionSettings.IsLogedAsAdmin(userId))
                {
                    ViewBag.Error = "No eres administrador y no tienes permiso para editarlo";
                    return null;
                }

                var collection = new Collection()
                {
                    Id = model["Collection.Id"],
                    Description = model["Collection.Description"],
                    Name = model["Collection.Name"],
                    Order = int.Parse(model["Collection.Order"])
                };
                collection.DrawingsReferences = await _drawingService.SetDrawingsReferences(EditCollectionsViewModel.ArrayDrawingsId(
                    model["DrawingList"]
                    ));

                var drawings = await _drawingService.GetAllDrawings();


                Collection result = null;
                if (!String.IsNullOrEmpty(collection.Id))
                {
                    result = await _drawingService.AddAsync(collection, drawings);
                    return result;
                }

                if (result == null)
                {
                    ViewBag.Error = "Ha ocurrido un error por el que no se ha guardado el dibujo";
                    return null;
                }

                _drawingService.CleanAllCache();
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
        public async Task<bool> RemoveCollection(string id)
        {
            return await _drawingService.RemoveCollection(id);
        }



        [AutorizacionRequerida]
        public async Task<IActionResult> NewCollection()
        {
            var list = await _drawingService.GetAllDrawings();
            var model = new EditCollectionsViewModel();
            model.Collection = new Collection();
            model.Collection.Drawings = new List<Drawing>();
            model.ListDrawings = list;
            return View("EditCollection", model);
        }
    }
}
