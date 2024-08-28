using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Globalization;

namespace MRA.DTO.Firebase.RemoteConfig
{
    public class RemoteConfigResponse
    {
        [JsonPropertyName("parameters")]
        public Dictionary<string, RemoteConfigParameter> Parameters { get; set; }
        [JsonPropertyName("parameterGroups")]
        public Dictionary<string, RemoteConfigParameterGroup> ParametersGroups { get; set; }

        [JsonPropertyName("version")]
        public RemoteConfigVersion Version { get; set; }

        private T ConvertParameter<T>(Dictionary<string, RemoteConfigParameter> parameters, RemoteConfigKey<T> key)
        {
            if (parameters.ContainsKey(key.Name))
            {
                var value = parameters[key.Name].DefaultValue?.Value;
                if (value != null)
                {
                    return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                }
            }
            return key.DefaultValue;
        }

        // Método para obtener el valor del parámetro, manejando claves anidadas
        public T GetParameter<T>(RemoteConfigKey<T> key)
        {
            if (String.IsNullOrEmpty(key.Group))
            {
                if (Parameters == null) return key.DefaultValue;
                return ConvertParameter(Parameters, key);
            }
            else
            {
                if (ParametersGroups == null) return key.DefaultValue;
                if (ParametersGroups.ContainsKey(key.Group))
                {
                    return ConvertParameter(ParametersGroups[key.Group].Parameters, key);
                }
            }
            return key.DefaultValue;
        }
    }

    public class RemoteConfigParameter
    {
        [JsonPropertyName("defaultValue")]
        public RemoteConfigDefaultValue DefaultValue { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("valueType")]
        public string ValueType { get; set; }

        // Subparámetros para manejar la estructura jerárquica
        [JsonPropertyName("subParameters")]
        public Dictionary<string, RemoteConfigParameter> SubParameters { get; set; }

        public override string ToString()
        {
            return $"{DefaultValue} ({ValueType})";
        }
    }


    public class RemoteConfigParameterGroup
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("parameters")]
        public Dictionary<string, RemoteConfigParameter> Parameters { get; set; }
    }

    public class RemoteConfigDefaultValue
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class RemoteConfigVersion
    {
        [JsonPropertyName("versionNumber")]
        public string VersionNumber { get; set; }

        [JsonPropertyName("updateTime")]
        public DateTime UpdateTime { get; set; }

        [JsonPropertyName("updateUser")]
        public RemoteConfigUpdateUser UpdateUser { get; set; }

        [JsonPropertyName("updateOrigin")]
        public string UpdateOrigin { get; set; }

        [JsonPropertyName("updateType")]
        public string UpdateType { get; set; }
    }

    public class RemoteConfigUpdateUser
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class RemoteConfigKey<T>
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public T DefaultValue { get; set; }

        public RemoteConfigKey(string group, string name, T defaultValue)
        {
            Group = group;
            Name = name;
            Type = typeof(T);
            DefaultValue = defaultValue;
        }
    }
}
