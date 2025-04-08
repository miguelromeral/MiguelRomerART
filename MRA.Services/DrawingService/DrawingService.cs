using Google.Cloud.Firestore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MRA.DTO.Firebase.Models;
using MRA.DTO.Options;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.ViewModels.Art.Select;
using MRA.Services.AzureStorage;
using MRA.Services.Firebase;
using MRA.Services.Firebase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services
{
    public class DrawingService : BaseCacheService, IDrawingService
    {
        private readonly IAzureStorageService _azureStorageService;
        private readonly IFirestoreService _firestoreService;
        private readonly ILogger _logger;
        private readonly RemoteConfigService _remoteConfigService;
        private readonly AppConfiguration _appConfiguration;

        private const string CACHE_ALL_DRAWINGS = "all_drawings";
        private const string CACHE_ALL_COLLECTIONS = "all_collections";

        public DrawingService(IMemoryCache cache, IAzureStorageService storageService, IFirestoreService firestoreService,
            RemoteConfigService remoteConfigService, ILogger logger, AppConfiguration appConfig) : base(cache)
        {
            _appConfiguration = appConfig;
            _logger = logger;
            _azureStorageService = storageService;
            _firestoreService = firestoreService;
            _remoteConfigService = remoteConfigService;
        }

        public async Task<List<Drawing>> GetAllDrawings()
        {
            return await GetOrSetAsync(CACHE_ALL_DRAWINGS, async () =>
            {
                return await _firestoreService.GetDrawingsAsync();
            }, TimeSpan.FromSeconds(_appConfiguration.Cache.RefreshSeconds));
        }

        public string GetAzureUrlBase() => _azureStorageService.GetBlobURL();
        public async Task<List<Inspiration>> GetAllInspirations()
        {
            return await GetOrSetAsync("all_inspirations", async () =>
            {
                return await _firestoreService.GetInspirationsAsync();
            }, TimeSpan.FromSeconds(_appConfiguration.Cache.RefreshSeconds));
        }

        public async Task<List<Collection>> GetAllCollections(List<Drawing> drawings, bool cache = true)
        {
            if (cache)
            {
                return await GetOrSetAsync(CACHE_ALL_COLLECTIONS, async () =>
                {
                    return await _firestoreService.GetCollectionsAsync(drawings);
                }, TimeSpan.FromSeconds(_appConfiguration.Cache.RefreshSeconds));
            }
            else
            {
                return await _firestoreService.GetCollectionsAsync(drawings);
            }
        }

        public async Task<Collection> FindCollectionById(string documentId, List<Drawing> drawings, bool cache = true)
        {
            if (cache)
            {
                return await GetOrSetAsync($"collection_{documentId}", async () =>
                {
                    return await _firestoreService.FindCollectionByIdAsync(documentId, drawings);
                }, TimeSpan.FromSeconds(_appConfiguration.Cache.RefreshSeconds));
            }
            else
            {
                return await _firestoreService.FindCollectionByIdAsync(documentId, drawings);
            }
        }

        public async Task RemoveCollection(string id)
        {
            await _firestoreService.RemoveCollectionAsync(id);
        }

        public async Task<FilterResults> FilterDrawingsGivenList(DrawingFilter filter, List<Drawing> drawings, List<Collection> collections)
        {
            return await GetOrSetAsync<FilterResults>(filter.CacheKey, async () =>
            {
                var results = await _firestoreService.FilterGivenList(filter, drawings, collections);
                var list = results.FilteredDrawings;
                SetBlobUrl(ref list);
                results.UpdatefilteredDrawings(list);
                return results;
            }, TimeSpan.FromSeconds(_appConfiguration.Cache.RefreshSeconds));
        }

        //public async Task<List<Drawing>> FilterDrawings(DrawingFilter filter)
        //{
        //    return await GetOrSetAsync<List<Drawing>>(filter.CacheKey, async () =>
        //    {
        //        List<Drawing> drawings = new List<Drawing>();
        //        drawings = await _firestoreService.Filter(filter);

        //        SetBlobUrl(ref drawings);

        //        switch (filter.Sortby)
        //        {
        //            //case "date-desc": 
        //            case "date-asc":
        //                drawings = drawings.OrderBy(x => x.Date).ToList();
        //                break;
        //            case "name-asc":
        //                drawings = drawings.OrderBy(x => x.Name).ToList();
        //                break;
        //            case "name-desc":
        //                drawings = drawings.OrderByDescending(x => x.Name).ToList();
        //                break;
        //            case "kudos-asc":
        //                drawings = drawings.OrderBy(x => x.Likes).ToList();
        //                break;
        //            case "kudos-desc":
        //                drawings = drawings.OrderByDescending(x => x.Likes).ToList();
        //                break;
        //            case "views-asc":
        //                drawings = drawings.OrderBy(x => x.Views).ToList();
        //                break;
        //            case "views-desc":
        //                drawings = drawings.OrderByDescending(x => x.Views).ToList();
        //                break;
        //            case "scorem-desc":
        //                drawings = drawings.Where(x => x.ScoreCritic > 0).OrderByDescending(x => x.ScoreCritic).ToList();
        //                break;
        //            case "scorem-asc":
        //                drawings = drawings.Where(x => x.ScoreCritic > 0).OrderBy(x => x.ScoreCritic).ToList();
        //                break;
        //            case "scoreu-desc":
        //                drawings = drawings.Where(x => x.ScorePopular > 0).OrderByDescending(x => (int)x.ScorePopular)
        //                    .ThenByDescending(x => x.VotesPopular).ToList();
        //                break;
        //            case "scoreu-asc":
        //                drawings = drawings.Where(x => x.ScorePopular > 0).OrderBy(x => (int)x.ScorePopular)
        //                    .ThenByDescending(x => x.VotesPopular).ToList();
        //                break;
        //            case "time-asc":
        //                drawings = drawings.Where(x => x.Time > 0).OrderBy(x => x.Time).ToList();
        //                break;
        //            case "time-desc":
        //                drawings = drawings.Where(x => x.Time > 0).OrderByDescending(x => x.Time).ToList();
        //                break;
        //            default:
        //                drawings = drawings.OrderByDescending(x => x.Date).ToList();
        //                break;
        //        }

        //        return drawings;
        //    }, TimeSpan.FromSeconds(_secondsCache));
        //}


        private void SetBlobUrl(ref List<Drawing> drawings)
        {
            drawings.ForEach(d => d.UrlBase = _azureStorageService.GetBlobURL());
        }

        public async Task<Drawing> FindDrawingById(string documentId, bool onlyIfVisible, bool updateViews = false, bool cache = true)
        {
            //await _firestoreService.UpdateViews(documentId);
            if (cache)
            {
                return await GetOrSetAsync<Drawing>($"drawing_{documentId}", async () =>
                {
                    return await FindDrawingByIdTask(documentId, onlyIfVisible, updateViews, cache);
                }, TimeSpan.FromSeconds(_appConfiguration.Cache.RefreshSeconds));
            }
            else
            {
                return await FindDrawingByIdTask(documentId, onlyIfVisible, updateViews, cache);
            }
        }

        private async Task<Drawing> FindDrawingByIdTask(string documentId, bool onlyIfVisible, bool updateViews = false, bool cache = true)
        {
            var result = await _firestoreService.FindDrawingByIdAsync(documentId, updateViews);
            if (result != null && onlyIfVisible && !result.Visible)
            {
                return null;
            }
            return result;
        }

        public async Task<bool> UpdateViews(string documentId) => await _firestoreService.UpdateViewsAsync(documentId);


        public async Task<bool> UpdateLikes(string documentId) => await _firestoreService.UpdateLikesAsync(documentId);
        public async Task<VoteSubmittedModel> Vote(string documentId, int score) => await _firestoreService.VoteAsync(documentId, score);

        public async Task<bool> ExistsBlob(string rutaBlob) => await _azureStorageService.ExistsBlob(rutaBlob);

        public async Task<Drawing> AddAsync(Drawing document)
        {
            Drawing current = await FindDrawingById(document.Id, false, false, false);
            if (current != null)
            {
                document.Likes = current.Likes;
                document.ScorePopular = current.ScorePopular;
                document.Views = current.Views;
                document.VotesPopular = current.VotesPopular;
            }
            return await _firestoreService.AddDrawingAsync(document);
        }

        public async Task<Collection> AddAsync(Collection document, List<Drawing> drawings) => await _firestoreService.AddCollectionAsync(document, drawings);

        public async Task RedimensionarYGuardarEnAzureStorage(string rutaEntrada, string nombreBlob, int anchoDeseado) =>
            await _azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, nombreBlob, anchoDeseado);

        public async Task RedimensionarYGuardarEnAzureStorage(MemoryStream rutaEntrada, string nombreBlob, int anchoDeseado) =>
            await _azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, nombreBlob, anchoDeseado);

        public string CrearThumbnailName(string rutaImagen) => _azureStorageService.CrearThumbnailName(rutaImagen);

        //public DocumentReference GetDbDocumentDrawing(string id) => _firestoreService.GetDbDocumentDrawing(id);


        public async Task<List<DocumentReference>> SetDrawingsReferences(string[] ids) => await _firestoreService.SetDrawingsReferencesAsync(ids);


        public List<ProductListItem> GetProducts(List<Drawing> drawings)
        {
            var list = new List<ProductListItem>();
            _logger.LogTrace("Dada la lista de dibujos, vamos a separar los productos");
            foreach (var product in drawings.Where(x => !string.IsNullOrEmpty(x.ProductName)).Select(x => new { x.ProductName, x.ProductType, x.ProductTypeName }).Distinct().ToList())
            {
                if (list.Count(x => x.ProductName == product.ProductName) == 0)
                {
                    list.Add(new ProductListItem()
                    {
                        ProductName = product.ProductName,
                        ProductTypeId = product.ProductType,
                        ProductType = product.ProductTypeName
                    });
                }
            }

            return list;
        }
        public List<CharacterListItem> GetCharacters(List<Drawing> drawings)
        {
            var list = new List<CharacterListItem>();

            foreach (var character in drawings.Where(x => !string.IsNullOrEmpty(x.ProductName)).Select(x => new { x.Name, x.ProductType, x.ProductTypeName }).Distinct().ToList())
            {
                if (list.Count(x => x.CharacterName == character.Name) == 0)
                {
                    list.Add(new CharacterListItem()
                    {
                        CharacterName = character.Name,
                        ProductTypeId = character.ProductType,
                        ProductType = character.ProductTypeName
                    });
                }
            }

            return list;
        }

        public List<string> GetModels(List<Drawing> drawings)
        {
            var list = new List<string>();

            foreach (var modelName in drawings.Where(x => !string.IsNullOrEmpty(x.ModelName)).Select(x => x.ModelName).Distinct().ToList())
            {
                if (!list.Contains(modelName))
                {
                    list.Add(modelName);
                }
            }

            return list;
        }
    }
}
