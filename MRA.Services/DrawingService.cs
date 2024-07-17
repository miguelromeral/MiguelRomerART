using Google.Cloud.Firestore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MRA.DTO.ViewModels.Art.Select;
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

        private readonly string CACHE_ALL_DRAWINGS = "all_drawings";
        private readonly string CACHE_ALL_COLLECTIONS = "all_collections";

        public DrawingService(int secondsCache, IMemoryCache cache, AzureStorageService storageService, IFirestoreService firestoreService) : base(cache)
        {
            _secondsCache = secondsCache;
            _azureStorageService = storageService;
            _firestoreService = firestoreService;
        }

        public async Task<List<Drawing>> GetAllDrawings()
        {
            return await GetOrSetAsync<List<Drawing>>(CACHE_ALL_DRAWINGS, async () =>
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

        public async Task<List<Collection>> GetAllCollections(bool cache = true)
        {
            if (cache)
            {
                return await GetOrSetAsync<List<Collection>>(CACHE_ALL_COLLECTIONS, async () =>
                {
                    return await _firestoreService.GetAllCollections();
                }, TimeSpan.FromSeconds(_secondsCache));
            }
            else
            {
                return await _firestoreService.GetAllCollections();
            }
        }

        public void CleanAllCache()
        {
            //CleanCacheItem(CACHE_ALL_DRAWINGS);
            //CleanCacheItem(CACHE_ALL_COLLECTIONS);
            base.CleanAllCache();

        }

        public async Task<Collection> FindCollectionById(string documentId, bool cache = true)
        {
            if (cache)
            {
                return await GetOrSetAsync<Collection>($"collection_{documentId}", async () =>
                {
                    return await _firestoreService.FindCollectionById(documentId);
                }, TimeSpan.FromSeconds(_secondsCache));
            }
            else
            {
                return await _firestoreService.FindCollectionById(documentId);
            }
        }

        public async Task<bool> RemoveCollection(string id)
        {
            try
            {
                await _firestoreService.RemoveCollection(id);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }


        public async Task<List<Collection>> GetAllCollectionsOrderPositive()
        {
            return await GetOrSetAsync<List<Collection>>("all_collections_positive", async () =>
            {
                return await _firestoreService.GetAllCollectionsOrderPositive();
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
                    case "scorem-desc":
                        drawings = drawings.Where(x => x.ScoreCritic > 0).OrderByDescending(x => x.ScoreCritic).ToList();
                        break;
                    case "scorem-asc":
                        drawings = drawings.Where(x => x.ScoreCritic > 0).OrderBy(x => x.ScoreCritic).ToList();
                        break;
                    case "scoreu-desc":
                        drawings = drawings.Where(x => x.ScorePopular > 0).OrderByDescending(x => (int) x.ScorePopular)
                            .ThenByDescending(x => x.VotesPopular).ToList();
                        break;
                    case "scoreu-asc":
                        drawings = drawings.Where(x => x.ScorePopular > 0).OrderBy(x => (int) x.ScorePopular)
                            .ThenByDescending(x => x.VotesPopular).ToList();
                        break;
                    case "time-asc":
                        drawings = drawings.Where(x => x.Time > 0).OrderBy(x => x.Time).ToList();
                        break;
                    case "time-desc":
                        drawings = drawings.Where(x => x.Time > 0).OrderByDescending(x => x.Time).ToList();
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

        public async Task<Drawing> FindDrawingById(string documentId, bool updateViews = false, bool cache = true)
        {
            //await _firestoreService.UpdateViews(documentId);
            if (cache)
            {
                return await GetOrSetAsync<Drawing>($"drawing_{documentId}", async () =>
                {
                    return await _firestoreService.FindDrawingById(documentId, updateViews);
                }, TimeSpan.FromSeconds(_secondsCache));
            }
            else
            {
                return await _firestoreService.FindDrawingById(documentId, updateViews);
            }
        }

        public async Task UpdateViews(string documentId) => await _firestoreService.UpdateViews(documentId);


        public async Task UpdateLikes(string documentId) => await _firestoreService.UpdateLikes(documentId);
        public async Task<VoteSubmittedModel> Vote(string documentId, int score) => await _firestoreService.Vote(documentId, score);

        public async Task<bool> ExistsBlob(string rutaBlob) => await _azureStorageService.ExistsBlob(rutaBlob);

        public async Task<Drawing> AddAsync(Drawing document) => await _firestoreService.AddAsync(document);

        public async Task<Collection> AddAsync(Collection document) => await _firestoreService.AddAsync(document);

        public async Task RedimensionarYGuardarEnAzureStorage(string rutaEntrada, string nombreBlob, int anchoDeseado) =>
            await _azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, nombreBlob, anchoDeseado);

        public async Task RedimensionarYGuardarEnAzureStorage(MemoryStream rutaEntrada, string nombreBlob, int anchoDeseado) =>
            await _azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, nombreBlob, anchoDeseado);

        public string CrearThumbnailName(string rutaImagen) => _azureStorageService.CrearThumbnailName(rutaImagen);

        public DocumentReference GetDbDocumentDrawing(string id) => _firestoreService.GetDbDocumentDrawing(id);


        public async Task<Resume> GetAllExperience()
        {
            return await GetOrSetAsync<Resume> ("all_experience", async () =>
            {
                return await _firestoreService.GetAllExperience();
            }, TimeSpan.FromSeconds(_secondsCache));
        }


        public async Task<List<DocumentReference>> SetDrawingsReferences(string[] ids) => await _firestoreService.SetDrawingsReferences(ids);


        public List<ProductListItem> GetProducts(List<Drawing> drawings)
        {
            var list = new List<ProductListItem>();

            foreach (var product in drawings.Where(x => !String.IsNullOrEmpty(x.ProductName)).Select(x => new { x.ProductName, x.ProductType, x.ProductTypeName }).Distinct().ToList())
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

            foreach (var character in drawings.Where(x => !String.IsNullOrEmpty(x.ProductName)).Select(x => new { x.Name, x.ProductType, x.ProductTypeName }).Distinct().ToList())
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

            foreach (var modelName in drawings.Where(x => !String.IsNullOrEmpty(x.ModelName)).Select(x => x.ModelName).Distinct().ToList())
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
