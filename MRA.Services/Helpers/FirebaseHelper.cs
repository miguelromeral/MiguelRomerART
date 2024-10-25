using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Helpers
{
    public static class FirebaseHelper
    {
        public static void SetCredentialsLocally(string credentialsFilePath)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsFilePath);
        }
    }
}
