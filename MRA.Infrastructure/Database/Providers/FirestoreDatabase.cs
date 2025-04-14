using Google.Cloud.Firestore;
using MRA.Infrastructure.Settings;
using MRA.Infrastructure.Database.Documents.Interfaces;
using MRA.Infrastructure.Database.Providers.Interfaces;

namespace MRA.Infrastructure.Database.Providers
{
    public class FirestoreDatabase : IDocumentsDatabase
    {
        private const string ENV_GOOGLE_CREDENTIALS_AZURE = "GOOGLE_APPLICATION_CREDENTIALS_JSON";
        private const string ENV_GOOGLE_CREDENTIALS = "GOOGLE_APPLICATION_CREDENTIALS";

        private readonly string _projectId;
        private string _serviceAccountPath = "";

        private FirestoreDb _firestoreDb;
        private FirestoreDb FirestoreDb
        {
            get
            {
                if (_firestoreDb == null)
                    InitializeFirestore();

                return _firestoreDb;
            }
        }

        public FirestoreDatabase(AppSettings appConfig)
        {
            _projectId = appConfig.Firebase.ProjectID;
            _serviceAccountPath = appConfig.Firebase.CredentialsPath;
        }


        public void InitializeFirestore()
        {
            LoadCredentials();
            Create();
        }

        private void LoadCredentials()
        {
            var googleCredentialsJson = Environment.GetEnvironmentVariable(ENV_GOOGLE_CREDENTIALS_AZURE);
            if (!string.IsNullOrEmpty(googleCredentialsJson))
            {
                var tempCredentialPath = Path.Combine(Path.GetTempPath(), "firebase-credentials.json");
                File.WriteAllText(tempCredentialPath, googleCredentialsJson);

                _serviceAccountPath = tempCredentialPath;
            }

            Environment.SetEnvironmentVariable(ENV_GOOGLE_CREDENTIALS, _serviceAccountPath);
        }

        private void Create()
        {
            _firestoreDb = FirestoreDb.Create(_projectId);
        }

        public async Task<IEnumerable<IDocument>> GetAllDocumentsAsync<IDocument>(string collection) =>
            (await FirestoreDb.Collection(collection).GetSnapshotAsync())
                .Documents.Select(s => s.ConvertTo<IDocument>());

        public async Task<bool> DocumentExistsAsync(string collection, string documentId)
        {
            DocumentReference docRef = FirestoreDb.Collection(collection).Document(documentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists;
        }

        public async Task<IDocument> GetDocumentAsync<IDocument>(string collection, string documentId)
        {
            DocumentReference docRef = FirestoreDb.Collection(collection).Document(documentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.ConvertTo<IDocument>();
        }

        public async Task<bool> SetDocumentAsync(string collection, string documentId, IDocument document)
        {
            try
            {
                DocumentReference docRef = FirestoreDb.Collection(collection).Document(documentId);
                await docRef.SetAsync(document);
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteDocumentAsync(string collection, string id)
        {
            try
            {
                DocumentReference docRef = FirestoreDb.Collection(collection).Document(id);
                await docRef.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
