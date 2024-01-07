using Microsoft.Extensions.Logging;
using MRA.Services.AzureStorage;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.Models;
using MRA.Web.Models.Art;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services
{
    public class DrawingService
    {

        private readonly AzureStorageService _azureStorageService;
        private readonly IFirestoreService _firestoreService;

        public DrawingService(AzureStorageService storageService, IFirestoreService firestoreService)
        {
            _azureStorageService = storageService;
            _firestoreService = firestoreService;
        }

        public async Task<List<Drawing>> GetAllDrawings()
        {
            var drawings = await _firestoreService.GetAll();
            SetBlobUrl(ref drawings);
            return drawings;
        }


        public string GetAzureUrlBase() => _azureStorageService.BlobURL;
        public async Task<List<Inspiration>> GetAllInspirations() => await _firestoreService.GetAllInspirations();
        public async Task<List<Collection>> GetAllCollections() => await _firestoreService.GetAllCollections();

        public async Task<List<Drawing>> FilterDrawings(FilterDrawingModel filter)
        {
            List<Drawing> drawings = new List<Drawing>();
            drawings = await _firestoreService.Filter(filter);

            SetBlobUrl(ref drawings);

            switch (filter.Sortby){
                //case "date-desc": 
                case "date-asc": 
                    drawings = drawings.OrderBy(x => x.Date).ToList();
                    break;
                case "name-asc":
                    drawings = drawings.OrderBy(x => x.Name).ToList();
                    break;
                case "name-desc":
                    drawings = drawings.OrderByDescending(x => x.Name).ToList();
                    break;
                case "kudos-asc":
                    drawings = drawings.OrderBy(x => x.Likes).ToList();
                    break;
                case "kudos-desc":
                    drawings = drawings.OrderByDescending(x => x.Likes).ToList();
                    break;
                case "views-asc":
                    drawings = drawings.OrderBy(x => x.Views).ToList();
                    break;
                case "views-desc":
                    drawings = drawings.OrderByDescending(x => x.Views).ToList();
                    break;
                default:
                    drawings = drawings.OrderByDescending(x => x.Date).ToList();
                    break;
            }

            return drawings;
        }

        private void SetBlobUrl(ref List<Drawing> drawings)
        {
            drawings.ForEach(d => d.UrlBase = _azureStorageService.BlobURL);
        }

        public async Task<Drawing> FindDrawingById(string documentId) => await _firestoreService.FindDrawingById(documentId);
        public async Task UpdateLikes(string documentId) => await _firestoreService.UpdateLikes(documentId);

        public async Task<bool> ExistsBlob(string rutaBlob) => await _azureStorageService.ExistsBlob(rutaBlob);

        public async Task<Drawing> AddAsync(Drawing document) => await _firestoreService.AddAsync(document);

        public async Task RedimensionarYGuardarEnAzureStorage(string rutaEntrada, string nombreBlob, int anchoDeseado) =>
            await _azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, nombreBlob, anchoDeseado);

        public string CrearThumbnailName(string rutaImagen) => _azureStorageService.CrearThumbnailName(rutaImagen);
    }
}
