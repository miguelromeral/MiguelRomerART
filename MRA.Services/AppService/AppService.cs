using Microsoft.Extensions.Logging;
using MRA.Infrastructure.Settings;
using MRA.DTO.ViewModels.Art;
using MRA.Infrastructure.Enums;
using MRA.Services.Models.Inspirations;
using MRA.Services.Models.Collections;
using MRA.DTO.Models;
using MRA.Services.Models.Drawings;
using MRA.Services.RemoteConfig;
using MRA.Services.Cache;
using MRA.Services.Storage;
using MRA.DTO.Enums.Drawing;
using MRA.Infrastructure.Cache;
using MRA.DTO.Enums.DrawingFilter;

namespace MRA.Services
{
    public class AppService : CacheServiceBase, IAppService
    {
        private readonly IStorageService _storageService;
        private readonly IDrawingService _drawingService;
        private readonly IInspirationService _inspirationService;
        private readonly ICollectionService _collectionService;
        private readonly ILogger<AppService> _logger;
        private readonly IRemoteConfigService _remoteConfigService;

        public const string CACHE_ALL_DRAWINGS = "all_drawings";
        public const string CACHE_ALL_INSPIRATIONS = "all_inspirations";
        public const string CACHE_ALL_COLLECTIONS = "all_collections";

        public AppService(
            ICacheProvider cache, 
            IStorageService storageService, 
            IRemoteConfigService remoteConfigService,
            IDrawingService drawingService,
            IInspirationService inspirationService,
            ICollectionService collectionService,
            ILogger<AppService> logger,
            AppSettings appConfig) : base(appConfig, cache)
        {
            _logger = logger;
            _storageService = storageService;
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

        public async Task<IEnumerable<InspirationModel>> GetAllInspirations()
        {
            return await GetOrSetFromCacheAsync(CACHE_ALL_INSPIRATIONS, async () =>
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

        private static IEnumerable<CollectionModel> LinkDrawingsToCollections(IEnumerable<CollectionModel> collections, IEnumerable<DrawingModel> drawings)
        {
            var newCollections = new List<CollectionModel>();

            foreach (var collection in collections)
                newCollections.Add(LinkDrawingToCollection(collection, drawings));
            
            newCollections = newCollections.Where(c => c.Drawings.Any()).ToList();
            return newCollections;
        }

        private static CollectionModel LinkDrawingToCollection(CollectionModel collection, IEnumerable<DrawingModel> drawgins)
        {
            collection.Drawings = drawgins.Where(d => (collection.DrawingIds.Contains(d.Id)));
            return collection;
        }

        public async Task<CollectionModel> FindCollectionByIdAsync(string collectionId, bool onlyIfVisible, bool cache = true)
        {
            return await GetOrSetFromCacheAsync($"collection_{collectionId}", async () =>
            {
                return await FetchCollectionAndLinkDrawings(collectionId, onlyIfVisible: onlyIfVisible, cache: cache);
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
            
            drawings = FilterDrawings(filter, drawings, collections);

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
                drawings = SortDrawingsByFilter(filter, drawings);
            }
            SetBlobUrl(ref drawings);

            return new FilterResults(drawings, collections, filter);
        }

        private IEnumerable<DrawingModel> SortDrawingsByFilter(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            return filter.Sortby switch
            {
                DrawingFilterSortBy.Latest => drawings.SortByLatest(),
                DrawingFilterSortBy.Oldest => drawings.SortByOldest(),
                DrawingFilterSortBy.NameAZ => drawings.SortByNameAZ(),
                DrawingFilterSortBy.NameZA => drawings.SortByNameZA(),
                DrawingFilterSortBy.LikeAscending => drawings.SortByLikeAscending(),
                DrawingFilterSortBy.LikeDescending => drawings.SortByLikeDescending(),
                DrawingFilterSortBy.ViewsAscending => drawings.SortByViewsAscending(),
                DrawingFilterSortBy.ViewsDescending => drawings.SortByViewsDescending(),
                DrawingFilterSortBy.AuthorScoreAscending => drawings.SortByAuthorScoreAscending(),
                DrawingFilterSortBy.AuthorScoreDescending => drawings.SortByAuthorScoreDescending(),
                DrawingFilterSortBy.UserScoreAscending => drawings.SortByUserScoreAscending(),
                DrawingFilterSortBy.UserScoreDescending => drawings.SortByUserScoreDescending(),
                DrawingFilterSortBy.Fastest => drawings.SortByFastest(),
                DrawingFilterSortBy.Slowest => drawings.SortBySlowest(),
                _ => CalculatePopularityOfListDrawings(drawings).SortByPopularity()
            };
        }

        private IEnumerable<DrawingModel> FilterDrawings(DrawingFilter filter, IEnumerable<DrawingModel> drawings, IEnumerable<CollectionModel> collections)
        {
            drawings = FilterDrawings_OnlyVisible(filter, drawings);
            drawings = FilterDrawings_OnlyFavorites(filter, drawings);
            drawings = FilterDrawings_TextQuery(filter, drawings);
            drawings = FilterDrawings_ProductName(filter, drawings);
            drawings = FilterDrawings_CharacterName(filter, drawings);
            drawings = FilterDrawings_ModelName(filter, drawings);
            drawings = FilterDrawings_Type(filter, drawings);
            drawings = FilterDrawings_ProductType(filter, drawings);
            drawings = FilterDrawings_Software(filter, drawings);
            drawings = FilterDrawings_Paper(filter, drawings);
            drawings = FilterDrawings_Spotify(filter, drawings);
            drawings = FilterDrawings_Collection(filter, drawings, collections);
            return drawings;
        }

        private static IEnumerable<DrawingModel> FilterDrawings_Collection(DrawingFilter filter, IEnumerable<DrawingModel> drawings, IEnumerable<CollectionModel> collections)
        {
            if (!String.IsNullOrEmpty(filter.Collection))
            {
                var collection = collections.FirstOrDefault(x => x.Id.Equals(filter.Collection));

                if (collection != null)
                {
                    var idsCollection = collection.Drawings.Select(x => x.Id).ToList();
                    drawings = drawings.Where(d => idsCollection.Contains(d.Id)).ToList();
                }
            }

            return drawings;
        }

        private static IEnumerable<DrawingModel> FilterDrawings_Spotify(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            if (filter.Spotify != null)
            {
                if (filter.Spotify ?? false)
                {
                    drawings = drawings.Where(x => !string.IsNullOrEmpty(x.SpotifyUrl)).ToList();
                }
                else
                {
                    drawings = drawings.Where(x => string.IsNullOrEmpty(x.SpotifyUrl)).ToList();
                }
            }

            return drawings;
        }

        private static IEnumerable<DrawingModel> FilterDrawings_Paper(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            if (filter.Paper != EnumExtensions.GetDefaultValue<DrawingPaperSizes>())
            {
                drawings = drawings.Where(x => x.Paper == filter.Paper).ToList();
            }

            return drawings;
        }

        private static IEnumerable<DrawingModel> FilterDrawings_Software(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            if (filter.Software != EnumExtensions.GetDefaultValue<DrawingSoftwares>())
            {
                drawings = drawings.Where(x => x.Software == filter.Software).ToList();
            }

            return drawings;
        }

        private static IEnumerable<DrawingModel> FilterDrawings_ProductType(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            if (filter.ProductType != EnumExtensions.GetDefaultValue<DrawingProductTypes>())
            {
                drawings = drawings.Where(x => x.ProductType == filter.ProductType).ToList();
            }

            return drawings;
        }

        private static IEnumerable<DrawingModel> FilterDrawings_Type(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            if (filter.Type != EnumExtensions.GetDefaultValue<DrawingTypes>())
            {
                drawings = drawings.Where(x => x.Type == filter.Type).ToList();
            }

            return drawings;
        }

        private static IEnumerable<DrawingModel> FilterDrawings_ModelName(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            if (!String.IsNullOrEmpty(filter.ModelName))
            {
                if (filter.ModelName.Equals(DrawingFilter.NoModel))
                {
                    drawings = drawings.Where(x =>
                        string.IsNullOrEmpty(x.ModelName)).ToList();
                }
                else
                {
                    drawings = drawings.Where(x =>
                        x.ModelName.Contains(filter.ModelName)).ToList();

                }
            }

            return drawings;
        }

        private static IEnumerable<DrawingModel> FilterDrawings_CharacterName(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            if (!String.IsNullOrEmpty(filter.CharacterName))
            {
                if (filter.CharacterName.Equals(DrawingFilter.NoCharacter))
                {
                    drawings = drawings.Where(x =>
                        string.IsNullOrEmpty(x.Name)).ToList();
                }
                else
                {
                    drawings = drawings.Where(x =>
                        x.Name.Contains(filter.CharacterName)).ToList();

                }
            }

            return drawings;
        }

        private static IEnumerable<DrawingModel> FilterDrawings_ProductName(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            if (!String.IsNullOrEmpty(filter.ProductName))
            {
                if (filter.ProductName.Equals(DrawingFilter.NoProduct))
                {
                    drawings = drawings.Where(x =>
                        string.IsNullOrEmpty(x.ProductName)).ToList();
                }
                else
                {
                    drawings = drawings.Where(x =>
                        x.ProductName.Contains(filter.ProductName)).ToList();

                }
            }

            return drawings;
        }

        private IEnumerable<DrawingModel> FilterDrawings_TextQuery(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            if (!String.IsNullOrEmpty(filter.TextQuery))
            {
                var tags = _drawingService.DeleteAndAdjustTags(filter.Tags).Select(x => x.ToLower());
                drawings = drawings.Where(x =>
                    x.Tags.Join(tags, t1 => t1.ToLower(), t2 => t2, (t1, t2) => t1.Contains(t2)).Any()).ToList();
            }

            return drawings;
        }

        private static IEnumerable<DrawingModel> FilterDrawings_OnlyFavorites(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            if (filter.Favorites)
            {
                drawings = drawings.Where(x => x.Favorite).ToList();
            }

            return drawings;
        }

        private static IEnumerable<DrawingModel> FilterDrawings_OnlyVisible(DrawingFilter filter, IEnumerable<DrawingModel> drawings)
        {
            if (filter.OnlyVisible)
            {
                drawings = drawings.Where(x => x.Visible).ToList();
            }

            return drawings;
        }

        private void SetBlobUrl(ref IEnumerable<DrawingModel> drawings)
        {
            foreach (var d in drawings)
            {
                d.UrlBase = _storageService.GetBlobURL();
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
    }
}
