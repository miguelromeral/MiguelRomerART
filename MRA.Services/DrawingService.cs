using Google.Cloud.Firestore;
using Microsoft.Extensions.Caching.Memory;
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
    public class DrawingService : BaseCacheService
    {
        private readonly int _secondsCache;
        private readonly AzureStorageService _azureStorageService;
        private readonly IFirestoreService _firestoreService;

        public DrawingService(int secondsCache, IMemoryCache cache, AzureStorageService storageService, IFirestoreService firestoreService) : base(cache)
        {
            _secondsCache = secondsCache;
            _azureStorageService = storageService;
            _firestoreService = firestoreService;
        }

        public async Task<List<Drawing>> GetAllDrawings()
        {
            return await GetOrSetAsync<List<Drawing>>("all_drawings", async () =>
            {
                return await FilterDrawings(FilterDrawingModel.GetModelNoFilters());
            }, TimeSpan.FromSeconds(_secondsCache));
        }

        public string GetAzureUrlBase() => _azureStorageService.BlobURL;
        public async Task<List<Inspiration>> GetAllInspirations()
        {
            return await GetOrSetAsync<List<Inspiration>>("all_inspirations", async () =>
            {
                return await _firestoreService.GetAllInspirations();
            }, TimeSpan.FromSeconds(_secondsCache));
        }

        public async Task<List<Collection>> GetAllCollections()
        {
            return await GetOrSetAsync<List<Collection>>("all_collections", async () =>
            {
                return await _firestoreService.GetAllCollections();
            }, TimeSpan.FromSeconds(_secondsCache));
        }


        public async Task<List<Drawing>> FilterDrawings(FilterDrawingModel filter)
        {
            return await GetOrSetAsync<List<Drawing>>(filter.CacheKey, async () => {
                List<Drawing> drawings = new List<Drawing>();
                drawings = await _firestoreService.Filter(filter);

                SetBlobUrl(ref drawings);

                switch (filter.Sortby)
                {
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
            }, TimeSpan.FromSeconds(_secondsCache));
        }


        private void SetBlobUrl(ref List<Drawing> drawings)
        {
            drawings.ForEach(d => d.UrlBase = _azureStorageService.BlobURL);
        }

        public async Task<Drawing> FindDrawingById(string documentId, bool cache = true)
        {
            //await _firestoreService.UpdateViews(documentId);
            if (cache)
            {
                return await GetOrSetAsync<Drawing>($"drawing_{documentId}", async () =>
                {
                    return await _firestoreService.FindDrawingById(documentId);
                }, TimeSpan.FromSeconds(_secondsCache));
            }
            else
            {
                return await _firestoreService.FindDrawingById(documentId);
            }
        }

        public async Task UpdateViews(string documentId) => await _firestoreService.UpdateViews(documentId);


        public async Task UpdateLikes(string documentId) => await _firestoreService.UpdateLikes(documentId);

        public async Task<bool> ExistsBlob(string rutaBlob) => await _azureStorageService.ExistsBlob(rutaBlob);

        public async Task<Drawing> AddAsync(Drawing document) => await _firestoreService.AddAsync(document);
        public async Task<Collection> AddAsync(Collection document) => await _firestoreService.AddAsync(document);

        public async Task RedimensionarYGuardarEnAzureStorage(string rutaEntrada, string nombreBlob, int anchoDeseado) =>
            await _azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, nombreBlob, anchoDeseado);

        public string CrearThumbnailName(string rutaImagen) => _azureStorageService.CrearThumbnailName(rutaImagen);

        public DocumentReference GetDbDocumentDrawing(string id) => _firestoreService.GetDbDocumentDrawing(id);
    }
}
