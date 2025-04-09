using Google.Cloud.Firestore;
using MRA.DTO.Firebase.Documents;
using MRA.DTO.Firebase.Models;
using MRA.DTO.ViewModels.Art;
using MRA.DTO.Firebase.Converters;
using System.Diagnostics;
using MRA.Services.Firebase.Interfaces;
using MRA.DTO.Firebase.RemoteConfig;
using MRA.Services.Firebase.Firestore;
using Microsoft.Extensions.Logging;
using MRA.DTO.Configuration;
using MRA.Services.Firebase.RemoteConfig;

namespace MRA.Services.Firebase
{
    public class FirestoreService : IFirestoreService
    {
        private const string ENVIRONMENT_PRODUCTION = "production";
        private const string ENV_GOOGLE_CREDENTIALS_AZURE = "GOOGLE_APPLICATION_CREDENTIALS_JSON";
        private const string ENV_GOOGLE_CREDENTIALS = "GOOGLE_APPLICATION_CREDENTIALS";

        private readonly IFirestoreDatabase _firestoreDb;
        private readonly ILogger _logger;
        private string _serviceAccountPath = "";
        private DrawingFirebaseConverter _converterDrawing;
        private InspirationFirebaseConverter _converterInspiration;
        private CollectionFirebaseConverter _converterCollection;
        private IRemoteConfigService _remoteConfigService;
        private readonly AppConfiguration _appConfiguration;

        public string ProjectId { get => _appConfiguration.Firebase.ProjectID; }
        public string CollectionDrawings { get => _appConfiguration.Firebase.CollectionDrawings; }
        public string CollectionCollections { get => _appConfiguration.Firebase.CollectionCollections; }
        public string CollectionInspirations { get => _appConfiguration.Firebase.CollectionInspirations; }
        public bool IsInProduction { get => _appConfiguration.Firebase.Environment?.Equals(ENVIRONMENT_PRODUCTION) ?? false; } 
        public string CredentialsPath
        {
            get
            {
                if (String.IsNullOrEmpty(_serviceAccountPath))
                {
                    throw new Exception("The Google Credentials Path is empty. You should call FirebaseHelper.SetCredentials() first");
                }

                return _serviceAccountPath;
            }
        }

        public FirestoreService(
            AppConfiguration appConfig, 
            IFirestoreDatabase db,
            IRemoteConfigService remoteConfigService
            , ILogger logger)
        {
            _appConfiguration = appConfig;
            _logger = logger;
            LoadCredentials();
            _firestoreDb = db;
            _firestoreDb.Create();
            _remoteConfigService = remoteConfigService;
            _converterDrawing = new DrawingFirebaseConverter(_appConfiguration.AzureStorage.BlobPath);
            _converterInspiration = new InspirationFirebaseConverter();
            _converterCollection = new CollectionFirebaseConverter();
        }

        public void LoadCredentials()
        {
            try
            {
                _logger.LogInformation("Loading credentials for Google Firebase");
                _serviceAccountPath = "";

                // Si estás en Azure, crea el archivo temporal desde la variable de entorno
                var googleCredentialsJson = Environment.GetEnvironmentVariable(ENV_GOOGLE_CREDENTIALS_AZURE);
                if (!string.IsNullOrEmpty(googleCredentialsJson))
                {
                    _logger.LogInformation($"Config found in Azure ENV values. Creating temporal file with its content. Variable: '{ENV_GOOGLE_CREDENTIALS_AZURE}'");
                    var tempCredentialPath = Path.Combine(Path.GetTempPath(), "firebase-credentials.json");
                    File.WriteAllText(tempCredentialPath, googleCredentialsJson);

                    _serviceAccountPath = tempCredentialPath;
                }
                else
                {
                    _logger.LogInformation("Not found config in Azure ENV values, reading local file.");
                    _serviceAccountPath = _appConfiguration.Firebase.CredentialsPath;
                }

                _logger.LogInformation("Google Credentials file to be read from " + _serviceAccountPath);
                Environment.SetEnvironmentVariable(ENV_GOOGLE_CREDENTIALS, _serviceAccountPath);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "An error happened while loading Google Credentials.");
            }
        }

        public DocumentReference DocumentReference(string collection, string id)
        {
            return _firestoreDb.GetDocumentReference(collection, id);
        }

        public async Task<List<Drawing>> GetDrawingsAsync()
        {
            //_logger.LogTrace("Recuperando todos los dibujos desde Firestore");
            var documents = await _firestoreDb.GetAllDocumentsAsync<DrawingDocument>(CollectionDrawings);
            return documents.Select(_converterDrawing.ConvertToModel).ToList();
        }


        public async Task<List<Inspiration>> GetInspirationsAsync()
        {
            try
            {
                var inspdocs = (await _firestoreDb.GetAllDocumentsAsync<InspirationDocument>(CollectionInspirations));
            return inspdocs.Select(_converterInspiration.ConvertToModel).ToList();
            }catch(Exception ex)
            {
                Console.WriteLine("Error when getting inspirations: " + ex.Message);
                return new List<Inspiration>();
            }
        }


        public async Task<List<Collection>> GetCollectionsAsync(List<Drawing> drawings)
        {
            try
            {
                var collectionDocs = (await _firestoreDb.GetAllDocumentsAsync<CollectionDocument>(CollectionCollections));
                return HandleAllCollections(collectionDocs, drawings);
        }
            catch (Exception ex)
            {
                Console.WriteLine("Error when getting collections: " + ex.Message);
                return new List<Collection>();
            }
        }

        public Collection HandleCollection(CollectionDocument collection, List<Drawing> drawings)
        {
            var c = new Collection()
            {
                Id = collection.Id,
                Name = collection.name,
                Order = collection.order,
                Description = collection.description,
                Drawings = new List<Drawing>(),
                DrawingsReferences = collection.drawings
            };

            var documentosReferenciados = new List<DrawingDocument>();
            if (collection.drawings != null)
            {
                foreach (var reference in collection.drawings)
                {
                    var drawing = drawings.Find(x => x.Id == reference.Id);
                    if (drawing != null)
                    {
                        c.Drawings.Add(drawing);
                    }
                }
            }

            return c;
        }

        public async Task<List<DocumentReference>> SetDrawingsReferencesAsync(string[] ids)
        {
            var list = new List<DocumentReference>();

            if (ids != null)
            {
                foreach (var id in ids)
                {
                    var tmp = _firestoreDb.GetDocumentReference(CollectionDrawings, id);
                    list.Add(tmp);
                }
            }

            return list;
        }

        public List<Collection> HandleAllCollections(List<CollectionDocument> collectionDocs, List<Drawing> drawings)
        {
            try
            {
                var listCollections = new List<Collection>();

                foreach(var collection in collectionDocs)
                {
                    listCollections.Add(HandleCollection(collection, drawings));
                }
                return listCollections;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when getting collections: " + ex.Message);
                return new List<Collection>();
            }
        }

        public async Task<FilterResults> FilterGivenList(DrawingFilter filter, List<Drawing> drawings, List<Collection> collections)
        {
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
                var tags = DeleteAndAdjustTags(filter.Tags).Select(x => x.ToLower());
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
            if(filter.Type > -1)
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
            if(filter.Spotify != null)
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
                var collection = collections.Find(x => x.Id.Equals(filter.Collection));

                if (collection != null)
                {
                    var idsCollection = collection.Drawings.Select(x => x.Id).ToList();
                    drawings = drawings.Where(d => idsCollection.Contains(d.Id)).ToList();
                }
            }


            if (filter.OnlyFilterCollection())
            {
                var selectedCollection = collections.Find(x => x.Id == filter.Collection);
                if (selectedCollection != null) {
                    var list = new List<Drawing>();
                    foreach (var id in selectedCollection.Drawings.Select(x => x.Id)) {
                        var d = drawings.Find(x => x.Id == id);
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
                        drawings = (await CalculatePopularityOfListDrawings(drawings)).OrderByDescending(x => x.Popularity).ToList();
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


            results.UpdatefilteredDrawings(drawings);
            return results;
        }

        public async Task<List<Drawing>> CalculatePopularityOfListDrawings(List<Drawing> drawings)
        {
            double wDate = 0, wCritic = 0, wPopular = 0, wFavorite = 0;
            int wMonths = 0;

            if (_remoteConfigService != null)
            {
                wDate = await _remoteConfigService.GetConfigValueAsync(RemoteConfigKeys.PopularityDateWeight);
                wMonths = await _remoteConfigService.GetConfigValueAsync(RemoteConfigKeys.PopularityDateMonths);
                wCritic = await _remoteConfigService.GetConfigValueAsync(RemoteConfigKeys.PopularityCriticWeight);
                wPopular = await _remoteConfigService.GetConfigValueAsync(RemoteConfigKeys.PopularityPopularWeight);
                wFavorite = await _remoteConfigService.GetConfigValueAsync(RemoteConfigKeys.PopularityFavoriteWeight);
            }

            foreach (var d in drawings)
            {
                d.CalculatePopularity(wDate, wMonths, wCritic, wPopular, wFavorite);
            }

            return drawings;
        }

        //public async Task<List<Drawing>> Filter(DrawingFilter filter)
        //{
        //    try
        //    {
        //        Query query = _firestoreDb.Collection(CollectionDrawings);

        //        //query = query.WhereNotEqualTo("visible", false);

        //        //if (filter.OnlyVisible)
        //        //{
        //        //    query = query.WhereEqualTo("visible", true);
        //        //}
        //        if (filter.Favorites)
        //        {
        //            query = query.WhereEqualTo("favorite", true);
        //        }
        //        if (!String.IsNullOrEmpty(filter.TextQuery))
        //        {
        //            query = query.WhereArrayContainsAny("tags", DeleteAndAdjustTags(filter.Tags));
        //        }
        //        if (!String.IsNullOrEmpty(filter.ProductName))
        //        {
        //            if (filter.ProductName.Equals("none"))
        //            {
        //                query = query.WhereEqualTo("product_name", "");
        //            }
        //            else
        //            {
        //                query = query.WhereEqualTo("product_name", filter.ProductName);
        //            }
        //        }
        //        if (!String.IsNullOrEmpty(filter.CharacterName))
        //        {
        //            if (filter.CharacterName.Equals("none"))
        //            {
        //                query = query.WhereEqualTo("name", "");
        //            }
        //            else
        //            {
        //                query = query.WhereEqualTo("name", filter.CharacterName);
        //            }
        //        }
        //        if (!String.IsNullOrEmpty(filter.ModelName))
        //        {
        //            if (filter.ModelName.Equals("none"))
        //            {
        //                query = query.WhereEqualTo("model_name", "");
        //            }
        //            else
        //            {
        //                query = query.WhereEqualTo("model_name", filter.ModelName);
        //            }
        //        }
        //        if (filter.Type > -1)
        //        {
        //            query = query.WhereEqualTo("type", filter.Type);
        //        }
        //        if (filter.ProductType > -1)
        //        {
        //            query = query.WhereEqualTo("product_type", filter.ProductType);
        //        }
        //        if (filter.Software > 0)
        //        {
        //            query = query.WhereEqualTo("software", filter.Software);
        //        }
        //        if (filter.Paper > 0)
        //        {
        //            query = query.WhereEqualTo("paper", filter.Paper);
        //        }
        //        if (filter.Spotify != null)
        //        {
        //            if (filter.Spotify ?? false)
        //            {
        //                query = query.WhereNotEqualTo("spotify_url", "");
        //            }
        //            else
        //            {
        //                query = query.WhereEqualTo("spotify_url", "");
        //            }
        //        }

        //        //// Aplicar la ordenación
        //        //switch (filter.Sortby)
        //        //{
        //        //    case "date-asc":
        //        //        query = query.OrderBy("date");
        //        //        break;
        //        //    case "date-desc":
        //        //        query = query.OrderByDescending("date");
        //        //        break;
        //        //    case "name-asc":
        //        //        query = query.OrderBy("name");
        //        //        break;
        //        //    case "name-desc":
        //        //        query = query.OrderByDescending("name");
        //        //        break;
        //        //    case "kudos-asc":
        //        //        query = query.OrderBy("likes");
        //        //        break;
        //        //    case "kudos-desc":
        //        //        query = query.OrderByDescending("likes");
        //        //        break;
        //        //    case "views-asc":
        //        //        query = query.OrderBy("views");
        //        //        break;
        //        //    case "views-desc":
        //        //        query = query.OrderByDescending("views");
        //        //        break;
        //        //    case "scorem-asc":
        //        //        query = query.OrderBy("score_critic");
        //        //        break;
        //        //    case "scorem-desc":
        //        //        query = query.OrderByDescending("score_critic");
        //        //        break;
        //        //    case "scoreu-asc":
        //        //        query = query.OrderBy("score_popular").OrderByDescending("votes_popular");
        //        //        break;
        //        //    case "scoreu-desc":
        //        //        query = query.OrderByDescending("score_popular").OrderByDescending("votes_popular");
        //        //        break;
        //        //    case "time-asc":
        //        //        query = query.OrderBy("time");
        //        //        break;
        //        //    case "time-desc":
        //        //        query = query.OrderByDescending("time");
        //        //        break;
        //        //    default:
        //        //        query = query.OrderByDescending("date");
        //        //        break;
        //        //}

        //        if (filter.PageSize > 0 && filter.PageNumber > 0)
        //        {
        //            query = query.Limit(filter.PageSize);

        //            // Obtener el documento de inicio basado en la página anterior
        //            var previousDocumentsQuery = query.Limit((filter.PageNumber- 1) * filter.PageSize);
        //            var previousDocuments = await previousDocumentsQuery.GetSnapshotAsync();
        //            var lastDocumentFromPreviousPage = previousDocuments.Documents.LastOrDefault();
        //            if (lastDocumentFromPreviousPage != null)
        //            {
        //                query = query.StartAfter(lastDocumentFromPreviousPage);
        //            }
        //        }

        //        var documents = (await query.GetSnapshotAsync()).Documents.Select(s => s.ConvertTo<DrawingDocument>()).ToList();

        //        var list = documents.Select(_converterDrawing.ConvertToModel).ToList();


        //        if (!String.IsNullOrEmpty(filter.Collection))
        //        {
        //            var collection = (await GetCollectionsAsync(list)).Find(x => x.Id.Equals(filter.Collection));

        //            if(collection != null)
        //            {
        //                var idsCollection = collection.Drawings.Select(x => x.Id).ToList();

        //                list = list.Where(d => idsCollection.Contains(d.Id)).ToList();
        //            }
        //        }

        //        if (filter.OnlyVisible)
        //        {
        //            list = list.Where(x => x.Visible).ToList();
        //        }

        //        return list;
        //    }
        //    catch(Exception ex)
        //    {
        //        Debug.WriteLine("Error when filtering documents: " + ex.Message);
        //    }
        //    return new List<Drawing>();
        //}

        public async Task<Drawing> FindDrawingByIdAsync(string documentId, bool updateViews = false)
        {
            try
            {
                var drawing = await _firestoreDb.GetDocumentAsync<DrawingDocument>(CollectionDrawings, documentId);

                if (drawing == null)
                {
                    Debug.WriteLine($"Documento \"{documentId}\" no encontrado entre los dibujos");
                    throw new KeyNotFoundException($"Documento \"{documentId}\" no encontrado entre los dibujos");
                }

                if (updateViews)
                {
                    await UpdateViewsAsync(documentId);
                }

                return _converterDrawing.ConvertToModel(drawing);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when getting document '" + documentId + "': " + ex.Message);
            }
            return null;
        }

        public async Task<Inspiration> FindInspirationById(string documentId)
        {
            try
            {
                var inspiration = await _firestoreDb.GetDocumentAsync<InspirationDocument>(CollectionInspirations, documentId);

                if (inspiration == null)
                {
                    Debug.WriteLine($"Documento \"{documentId}\" no encontrado entre las inspiraciones");
                    throw new KeyNotFoundException($"Documento \"{documentId}\" no encontrado entre las inspiraciones");
                }

                var converter = new InspirationFirebaseConverter();
                return converter.ConvertToModel(inspiration);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when getting document '" + documentId + "': " + ex.Message);
            }
            return null;
        }

        public async Task<Collection> FindCollectionByIdAsync(string documentId, List<Drawing> drawings)
        {
            try
            {
                var collectionDocument = await _firestoreDb.GetDocumentAsync<CollectionDocument>(CollectionCollections, documentId);

                if (collectionDocument == null)
                {
                    Debug.WriteLine($"Documento \"{documentId}\" no encontrado entre las colecciones");
                    throw new KeyNotFoundException($"Documento \"{documentId}\" no encontrado entre las colecciones");
                }

                return HandleCollection(collectionDocument, drawings);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when getting document '" + documentId + "': " + ex.Message);
            }
            return null;
        }

        public async Task<Google.Cloud.Firestore.WriteResult> RemoveCollectionAsync(string id)
        {
            return await _firestoreDb.DeleteDocumentAsync(CollectionCollections, id);
        }

        public async Task<bool> UpdateViewsAsync(string documentId)
        {
            return await _firestoreDb.UpdateViewsAsync(CollectionDrawings, documentId);
        }

        public async Task<bool> UpdateLikesAsync(string documentId)
        {
            return await _firestoreDb.UpdateLikesAsync(CollectionDrawings, documentId);
        }

        public async Task<VoteSubmittedModel> VoteAsync(string documentId, int score)
        {
            return await _firestoreDb.VoteAsync(CollectionDrawings, documentId, score);
        }

        public void SetAutomaticTags(ref Drawing document)
        {
            var list = new List<string>();
            list.AddRange((document.Name ?? "").Split(" ").Select(x => x.ToLower()));
            list.AddRange((document.ModelName ?? "").Split(" ").Select(x => x.ToLower()));
            list.AddRange((document.Title ?? "").Split(" ").Select(x => x.ToLower()));
            if (document.Software > 0)
            {
                list.AddRange(document.SoftwareName.Split(" ").Select(x => x.ToLower()));
            }
            if (document.Paper > 0)
            {
                list.AddRange(document.PaperHuman.Split(" ").Select(x => x.ToLower()));
            }
            if (document.Type > 0)
            {
                list.AddRange(document.TypeName.Split(" ").Select(x => x.ToLower()));
            }

            if (document.ProductType > 0)
            {
                list.AddRange(document.ProductTypeName.Split(" ").Select(x => x.ToLower()));
            }
            list.AddRange(document.ProductName.Split(" ").Select(x => x.ToLower()));

            document.Tags.AddRange(list);
            document.Tags = DeleteAndAdjustTags(document.Tags);
        }

        private List<string> DeleteAndAdjustTags(List<string> tags)
        {
            var eliminar = new List<string>()
            {
                "a",
                "un",
                "unas",
                "unos",
                "uno",
                "de",
                "el",
                "la",
                "los",
                "los",
                "les",
                "the"
            };
            var processed = tags.Select(x => 
                x.ToLower()
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace("ä", "a")
                .Replace("ë", "e")
                .Replace("ï", "i")
                .Replace("ö", "o")
                .Replace("ü", "u")
                .Replace(":", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("?", "")
                .Replace("¿", "")
                .Replace("/", "")
                .Replace("`", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("'", "")
                .Replace("@", " ")
                .Replace("#", " ")
                .Replace("!", "")
                .Replace("¡", "")
                .Replace("~", "")
                .Replace("$", " ")
                .Replace("%", " ")
                .Replace("&", " ")
                .Replace("\"", "")
                .Replace(" ", "")
                .Replace("_", " ")
                .Replace("-", " ")
            );

            var final = new List<string>() { };
            foreach(var s in processed)
            {
                foreach(var s2 in s.Split(" "))
                {
                    if (!eliminar.Contains(s2) && !String.IsNullOrEmpty(s2) && !s2.Equals(" "))
                    {
                        final.Add(s);
                    }
                }
            }

            return final.Distinct().ToList();
        }

        public async Task<Drawing> AddDrawingAsync(Drawing document)
        {
            SetAutomaticTags(ref document);
            
            var drawingDocument = _converterDrawing.ConvertToDocument(document);

            await _firestoreDb.SetDocumentAsync(CollectionDrawings, document.Id, drawingDocument);

            return _converterDrawing.ConvertToModel(drawingDocument);
        }


        public async Task<Collection> AddCollectionAsync(Collection document, List<Drawing> drawings)
        {
            var collectionDocument = _converterCollection.ConvertToDocument(document);

            await _firestoreDb.SetDocumentAsync(CollectionCollections, document.Id, collectionDocument);

            return HandleCollection(collectionDocument, drawings);
        }

        //public DocumentReference GetDbDocumentDrawing(string id)
        //{
        //    return _firestoreDb.Document(CollectionDrawings + "/" + id);
        //}


        public async Task AddInspirationAsync(Inspiration document)
        {
            var inspirationDocument = _converterInspiration.ConvertToDocument(document);

            await _firestoreDb.SetDocumentAsync(CollectionInspirations, document.Id, inspirationDocument);
        }
    }
}
