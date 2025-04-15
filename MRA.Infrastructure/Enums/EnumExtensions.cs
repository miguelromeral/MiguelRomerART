using System.ComponentModel;
using System.Reflection;

namespace MRA.Infrastructure.Enums;

public static class EnumExtensions
{
    public static string GetDescription<TEnum>(this TEnum enumValue) where TEnum : Enum
    {
        FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
        if (field != null)
        {
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? enumValue.ToString();
        }

        return enumValue.ToString();
    }

    public static TEnum GetDefaultValue<TEnum>() where TEnum : Enum
    {
        // Buscar el atributo DefaultEnumValue
        var attribute = typeof(TEnum)
            .GetCustomAttributes(typeof(DefaultEnumValueAttribute), false)
            .FirstOrDefault() as DefaultEnumValueAttribute;

        return attribute != null
            ? (TEnum)attribute.DefaultValue
            : default(TEnum); // Si no hay atributo, usar default
    }
}