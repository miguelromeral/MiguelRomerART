using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

namespace MRA.Services.Helpers
{
    public class FirebaseHelper
    {
        private ConfigurationHelper _configuration;

        private const string APPSETTING_FIREBASE_CREDENTIALS_PATH = "Firebase:CredentialsPath";
        private const string APPSETTING_FIREBASE_PROJECTID = "Firebase:ProjectID";
        private const string APPSETTING_FIREBASE_COLLECTION_DRAWINGS = "Firebase:CollectionDrawings";
        private const string APPSETTING_FIREBASE_COLLECTION_COLLECTIONS = "Firebase:CollectionCollections";
        private const string APPSETTING_FIREBASE_COLLECTION_INSPIRATIONS = "Firebase:CollectionInspirations";

        private const string ENV_GOOGLE_CREDENTIALS_AZURE = "GOOGLE_APPLICATION_CREDENTIALS_JSON";
        private const string ENV_GOOGLE_CREDENTIALS = "GOOGLE_APPLICATION_CREDENTIALS";
        
        private string _serviceAccountPath = "";

        public string ProjectId { get { return _configuration[APPSETTING_FIREBASE_PROJECTID]; } }
        public string CollectionDrawings { get { return _configuration[APPSETTING_FIREBASE_COLLECTION_DRAWINGS]; } }
        public string CollectionCollections { get { return _configuration[APPSETTING_FIREBASE_COLLECTION_COLLECTIONS]; } }
        public string CollectionInspirations { get { return _configuration[APPSETTING_FIREBASE_COLLECTION_INSPIRATIONS]; } }

        public FirebaseHelper(IConfiguration configuration)
        {
            _configuration = new ConfigurationHelper(configuration);
        }

        public void LoadCredentials()
        {
            _serviceAccountPath = "";

            // Si estás en Azure, crea el archivo temporal desde la variable de entorno
            var googleCredentialsJson = Environment.GetEnvironmentVariable(ENV_GOOGLE_CREDENTIALS_AZURE);
            if (!string.IsNullOrEmpty(googleCredentialsJson))
            {
                var tempCredentialPath = Path.Combine(Path.GetTempPath(), "firebase-credentials.json");
                File.WriteAllText(tempCredentialPath, googleCredentialsJson);

                _serviceAccountPath = tempCredentialPath;
            }
            else
            {
                // Si estoy en local
                _serviceAccountPath = _configuration[APPSETTING_FIREBASE_CREDENTIALS_PATH];
            }

            Environment.SetEnvironmentVariable(ENV_GOOGLE_CREDENTIALS, _serviceAccountPath);
        }

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
    }
}
