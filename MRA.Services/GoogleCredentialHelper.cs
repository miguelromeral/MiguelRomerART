using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services
{
    public static class GoogleCredentialHelper
    {
        public static async Task<string> GetAccessTokenAsync(string serviceAccountJsonPath)
        {
            GoogleCredential credential = GoogleCredential.FromFile(serviceAccountJsonPath)
                .CreateScoped(new[] { "https://www.googleapis.com/auth/firebase.remoteconfig" });

            return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        }
    }
}
