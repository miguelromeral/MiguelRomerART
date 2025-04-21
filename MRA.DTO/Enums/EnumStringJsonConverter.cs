using MRA.Infrastructure.Enums;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MRA.DTO.Enums;

public class EnumStringJsonConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();

            foreach (var field in typeof(TEnum).GetFields())
            {
                var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttribute != null && descriptionAttribute.Description == stringValue)
                {
                    return (TEnum)field.GetValue(null);
                }
            }

            if (Enum.TryParse(stringValue, true, out TEnum result))
            {
                return result;
            }
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            var intValue = reader.GetInt32();
            if (Enum.IsDefined(typeof(TEnum), intValue))
            {
                return (TEnum)Enum.ToObject(typeof(TEnum), intValue);
            }
        }

        return EnumExtensions.GetDefaultValue<TEnum>();
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}