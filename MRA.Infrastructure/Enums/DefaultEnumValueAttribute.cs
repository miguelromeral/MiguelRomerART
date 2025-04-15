namespace MRA.Infrastructure.Enums;

[AttributeUsage(AttributeTargets.Enum)]
public class DefaultEnumValueAttribute : Attribute
{
    public object DefaultValue { get; }

    public DefaultEnumValueAttribute(object defaultValue)
    {
        DefaultValue = defaultValue;
    }
}