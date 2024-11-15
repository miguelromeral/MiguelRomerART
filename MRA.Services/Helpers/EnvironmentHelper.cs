using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Helpers
{
    public static class EnvironmentHelper
    {
        public static string ReadValue(string key, bool required = true)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (required && String.IsNullOrEmpty(value))
            {
                throw new Exception($"No se ha especificado la variable de entorno \"{key}\".");
            }
            return value ?? "";
        }
    }
}
