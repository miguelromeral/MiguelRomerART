using MRA.Infrastructure.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MRA.DTO.Enums;

public class EnumStringJsonConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && Enum.TryParse(reader.GetString(), true, out TEnum result))
        {
            return result;
        }

        if (reader.TokenType == JsonTokenType.Number && Enum.IsDefined(typeof(TEnum), reader.GetInt32()))
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), reader.GetInt32());
        }

        //throw new JsonException($"No se puede convertir el valor '{reader.GetString()}' al tipo {typeof(TEnum).Name}.");
        //return (TEnum)(object)-1;
        return EnumExtensions.GetDefaultValue<TEnum>();
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}