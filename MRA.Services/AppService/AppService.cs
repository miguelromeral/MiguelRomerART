using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MRA.Infrastructure.Settings;
using MRA.DTO.ViewModels.Art;
using MRA.Services.AzureStorage;
using MRA.Services.Models.Inspirations;
using MRA.Services.Models.Collections;
using MRA.DTO.Models;
using MRA.Services.Models.Drawings;
using MRA.DTO.Firebase.RemoteConfig;
using MRA.Services.RemoteConfig;
using MRA.Services.Cache;

namespace MRA.Services
{
    public class AppService : CacheServiceBase, IAppService
    {
        private readonly IAzureStorageService _azureStorageService;
        private readonly IDrawingService _drawingService;
        private readonly IInspirationService _inspirationService;
        private readonly ICollectionService _collectionService;
        private readonly ILogger<AppService> _logger;
        private readonly IRemoteConfigService _remoteConfigService;
        private readonly AppSettings _appConfiguration;

        private const string CACHE_ALL_DRAWINGS = "all_drawings";
        private const string CACHE_ALL_COLLECTIONS = "all_collections";

        public AppService(
            IMemoryCache cache, 
            IAzureStorageService storageService, 
            IRemoteConfigService remoteConfigService,
            IDrawingService drawingService,
            IInspirationService inspirationService,
            ICollectionService collectionService,
            ILogger<AppService> logger,
            AppSettings appConfig) : base(appConfig, cache)
        {
            _appConfiguration = appConfig;
            _logger = logger;
            _azureStorageService = storageService;
            _remoteConfigService = remoteConfigService;
            _drawingService = drawingService;
            _inspirationService = inspirationService;
            _collectionService = collectionService;
        }

        public async Task<IEnumerable<DrawingModel>> GetAllDrawings(bool onlyIfVisible, bool cache = true)
        {
            return await GetOrSetFromCacheAsync(CACHE_ALL_DRAWINGS + onlyIfVisible, async () =>
                {
                    return await _drawingService.GetAllDrawingsAsync(onlyIfVisible);
                }, 
                useCache: cache);
        }

        public string GetAzureUrlBase() => _azureStorageService.GetBlobURL();
        public async Task<IEnumerable<InspirationModel>> GetAllInspirations()
        {
            return await GetOrSetFromCacheAsync("all_inspirations", async () =>
                {
                    return await _inspirationService.GetAllInspirationsAsync();
                }, 
                useCache: true);
        }

        public async Task<IEnumerable<CollectionModel>> GetAllCollectionsAsync(bool onlyIfVisible, bool cache = true)
        {
            return await GetOrSetFromCacheAsync(CACHE_ALL_COLLECTIONS + onlyIfVisible, async () =>
                {
                    return await FetchCollectionsAndLinkDrawings(onlyIfVisible, cache);
                }, 
                useCache: cache);
        }

        private async Task<IEnumerable<CollectionModel>> FetchCollectionsAndLinkDrawings(bool onlyIfVisible, bool cache)
        {
            var collections = await _collectionService.GetAllCollectionsAsync(onlyIfVisible);
            var drawgins = await GetAllDrawings(onlyIfVisible, cache);
            return LinkDrawingsToCollections(collections, drawgins);
        }

        private async Task<CollectionModel> FetchCollectionAndLinkDrawings(string id, bool onlyIfVisible, bool cache)
        {
            var collection = await _collectionService.FindCollectionAsync(id);
            var drawgins = await GetAllDrawings(onlyIfVisible, cache);
            return LinkDrawingToCollection(collection, drawgins);
        }

        private static IEnumerable<CollectionModel> LinkDrawingsToCollections(IEnumerable<CollectionModel> collections, IEnumerable<DrawingModel> drawgins)
        {
            var newCollections = new List<CollectionModel>();

            foreach (var collection in collections)
                newCollections.Add(LinkDrawingToCollection(collection, drawgins));
            
            newCollections = newCollections.Where(c => c.Drawings.Count() > 0).ToList();
            return newCollections;
        }

        private static CollectionModel LinkDrawingToCollection(CollectionModel collection, IEnumerable<DrawingModel> drawgins)
        {
            collection.Drawings = drawgins.Where(d => (collection?.DrawingIds?.Contains(d.Id) ?? false));
            return collection;
        }

        public async Task<CollectionModel> FindCollectionByIdAsync(string documentId, bool onlyIfVisible, bool cache = true)
        {
            return await GetOrSetFromCacheAsync($"collection_{documentId}", async () =>
            {
                return await FetchCollectionAndLinkDrawings(documentId, onlyIfVisible: onlyIfVisible, cache: cache);
            },
            useCache: cache);
        }


        public async Task<FilterResults> FilterDrawingsAsync(DrawingFilter filter)
        {
            return await GetOrSetFromCacheAsync(filter.CacheKey, async () =>
                {
                    return await FilterGivenList(filter);
                },
                useCache: true);
        }

        private async Task<FilterResults> FilterGivenList(DrawingFilter filter)
        {
            var drawings = await _drawingService.GetAllDrawingsAsync(filter.OnlyVisible);
            var collections = await _collectionService.GetAllCollectionsAsync(filter.OnlyVisible);
            collections = LinkDrawingsToCollections(collections, drawings);

            if (filter.OnlyVisible)
            {
                drawings = drawings.Where(x => x.Visible).ToList();
            }
            if (filter.Favorites)
            {
                drawings = drawings.Where(x => x.Favorite).ToList();
            }
            if (!String.IsNullOrEmpty(filter.TextQuery))
            {
                var tags = _drawingService.DeleteAndAdjustTags(filter.Tags).Select(x => x.ToLower());
                drawings = drawings.Where(x =>
                    x.Tags.Join(tags, t1 => t1.ToLower(), t2 => t2, (t1, t2) => t1.Contains(t2)).Any()).ToList();
            }
            if (!String.IsNullOrEmpty(filter.ProductName))
            {
                if (filter.ProductName.Equals("none"))
                {
                    drawings = drawings.Where(x =>
                        x.ProductName.Equals("")).ToList();
                }
                else
                {
                    drawings = drawings.Where(x =>
                        x.ProductName.Contains(filter.ProductName)).ToList();

                }
            }
            if (!String.IsNullOrEmpty(filter.CharacterName))
            {
                if (filter.CharacterName.Equals("none"))
                {
                    drawings = drawings.Where(x =>
                        x.Name.Equals("")).ToList();
                }
                else
                {
                    drawings = drawings.Where(x =>
                        x.Name.Contains(filter.CharacterName)).ToList();

                }
            }
            if (!String.IsNullOrEmpty(filter.ModelName))
            {
                if (filter.ModelName.Equals("none"))
                {
                    drawings = drawings.Where(x =>
                        x.ModelName.Equals("")).ToList();
                }
                else
                {
                    drawings = drawings.Where(x =>
                        x.ModelName.Contains(filter.ModelName)).ToList();

                }
            }
            if (filter.Type > -1)
            {
                drawings = drawings.Where(x => x.Type == filter.Type).ToList();
            }
            if (filter.ProductType > -1)
            {
                drawings = drawings.Where(x => x.ProductType == filter.ProductType).ToList();
            }
            if (filter.Software > 0)
            {
                drawings = drawings.Where(x => x.Software == filter.Software).ToList();
            }
            if (filter.Paper > 0)
            {
                drawings = drawings.Where(x => x.Paper == filter.Paper).ToList();
            }
            if (filter.Spotify != null)
            {

                if (filter.Spotify ?? false)
                {
                    drawings = drawings.Where(x => !x.SpotifyUrl.Equals("")).ToList();
                }
                else
                {
                    drawings = drawings.Where(x => x.SpotifyUrl.Equals("")).ToList();
                }
            }
            if (!String.IsNullOrEmpty(filter.Collection))
            {
                var collection = collections.FirstOrDefault(x => x.Id.Equals(filter.Collection));

                if (collection != null)
                {
                    var idsCollection = collection.Drawings.Select(x => x.Id).ToList();
                    drawings = drawings.Where(d => idsCollection.Contains(d.Id)).ToList();
                }
            }


            if (filter.OnlyFilterCollection())
            {
                var selectedCollection = collections.FirstOrDefault(x => x.Id == filter.Collection);
                if (selectedCollection != null)
                {
                    var list = new List<DrawingModel>();
                    foreach (var id in selectedCollection.Drawings.Select(x => x.Id))
                    {
                        var d = drawings.FirstOrDefault(x => x.Id == id);
                        if (d != null)
                        {
                            list.Add(d);
                        }
                    }
                    drawings = list;
                }
            }
            else
            {
                switch (filter.Sortby)
                {
                    case "date-asc":
                        drawings = drawings.OrderBy(x => x.Date).ToList();
                        break;
                    case "date-desc":
                        drawings = drawings.OrderByDescending(x => x.Date).ToList();
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
                    case "scorem-asc":
                        drawings = drawings.OrderBy(x => x.ScoreCritic).ThenBy(x => x.ScorePopular).ThenBy(x => x.VotesPopular).ToList();
                        break;
                    case "scorem-desc":
                        drawings = drawings.OrderByDescending(x => x.ScoreCritic).ThenByDescending(x => x.ScorePopular).ThenByDescending(x => x.VotesPopular).ToList();
                        break;
                    case "scoreu-asc":
                        drawings = drawings.Where(x => x.VotesPopular > 0).OrderBy(x => x.ScorePopular).ThenBy(x => x.VotesPopular).ToList();
                        break;
                    case "scoreu-desc":
                        drawings = drawings.Where(x => x.VotesPopular > 0).OrderByDescending(x => x.ScorePopular).ThenByDescending(x => x.VotesPopular).ToList();
                        break;
                    case "time-asc":
                        drawings = drawings.OrderBy(x => x.Time).ToList();
                        break;
                    case "time-desc":
                        drawings = drawings.OrderByDescending(x => x.Time).ToList();
                        break;
                    default:
                        drawings = CalculatePopularityOfListDrawings(drawings).OrderByDescending(x => x.Popularity).ToList();
                        break;
                }
            }

            var results = new FilterResults(drawings);
            var ids = drawings.Select(x => x.Id).ToList();
            results.FilteredCollections = collections
                .Where(c => c.Drawings.Any(d => ids.Contains(d.Id)))
                .Select(x => x.Id)
                .ToList();


            if (filter.PageSize > 0 && filter.PageNumber > 0)
            {
                // Saltar los elementos de las páginas anteriores
                drawings = drawings.Skip((filter.PageNumber - 1) * filter.PageSize)
                    // Tomar solo los elementos de la página actual
                    .Take(filter.PageSize)
                    .ToList();
            }


            //foreach (var d in drawings)
            //{
            //    Debug.WriteLine($"{d.Id.PadRight(30)} ({d.Name.PadRight(30)}): [{d.Popularity.ToString("#.###").PadRight(5)} == " +
            //        $"{d.PopularityDate.ToString("#.##").PadRight(5)} + {d.PopularityCritic.ToString("#.##").PadRight(5)} + {d.PopularityPopular.ToString("#.##").PadRight(5)}" +
            //        $" + {d.PopularityFavorite.ToString("#.##").PadRight(5)} ]");
            //}

            SetBlobUrl(ref drawings);

            results.UpdatefilteredDrawings(drawings);
            return results;
        }

        private void SetBlobUrl(ref IEnumerable<DrawingModel> drawings)
        {
            foreach (var d in drawings)
            {
                d.UrlBase = _azureStorageService.GetBlobURL();
            }
        }

        public IEnumerable<DrawingModel> CalculatePopularityOfListDrawings(IEnumerable<DrawingModel> drawings)
        {
            double wDate = _remoteConfigService.GetPopularityDate();
            int wMonths = _remoteConfigService.GetPopularityMonths();
            double wCritic = _remoteConfigService.GetPopularityCritic();
            double wPopular = _remoteConfigService.GetPopularityPopular();
            double wFavorite = _remoteConfigService.GetPopularityFavorite();

            foreach (var d in drawings)
            {
                d.CalculatePopularity(wDate, wMonths, wCritic, wPopular, wFavorite);
            }

            return drawings;
        }


        public async Task<DrawingModel> FindDrawingByIdAsync(string documentId, bool onlyIfVisible, bool updateViews = false, bool cache = true)
        {
            return await GetOrSetFromCacheAsync($"drawing_{documentId}", async () =>
            {
                return await _drawingService.FindDrawingAsync(documentId, onlyIfVisible, updateViews);
            },
            useCache: cache);
        }

        public async Task<bool> ExistsBlob(string rutaBlob) => await _azureStorageService.ExistsBlob(rutaBlob);

        public async Task<bool> AddAsync(CollectionModel model, List<DrawingModel> drawings)
        {
            return await _collectionService.SaveCollectionAsync(model.Id, model);
        }

        public async Task RedimensionarYGuardarEnAzureStorage(string rutaEntrada, string nombreBlob, int anchoDeseado) =>
            await _azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, nombreBlob, anchoDeseado);

        public async Task RedimensionarYGuardarEnAzureStorage(MemoryStream rutaEntrada, string nombreBlob, int anchoDeseado) =>
            await _azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, nombreBlob, anchoDeseado);

        public string CrearThumbnailName(string rutaImagen) => _azureStorageService.CrearThumbnailName(rutaImagen);
    }
}
