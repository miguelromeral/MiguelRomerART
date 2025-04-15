using System.Globalization;
using System.Reflection;

namespace MRA.Infrastructure.Excel.Attributes;

public class ExcelColumnInfo
{
    public PropertyInfo Property { get; set; }
    public ExcelColumnAttribute Attribute { get; set; }

    public override string ToString()
    {
        return $"{Attribute.Name} => {Property.PropertyType}";
    }

    public bool SameValues<T>(T object1, T object2)
    {
        var v1 = GetValue(object1);
        var v2 = GetValue(object2);

        if(v1 is null && v2 is null)
        {
            return true;
        }
        else if (v1 is null || v2 is null)
        {
            return false;
        }
        else if (v1 is List<string> list1 && v2 is List<string> list2)
        {
            if (list1.Count != list2.Count) return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i])) return false;
            }

            return true;
        }
        else
        {
            return v1.Equals(v2);
        }
    }

    public object? GetValue<T>(T? instance)
    {
        if (instance is null || Property == null) return null;

        return Property.GetValue(instance);
    }

    public string GetValueToPrint<T>(T instance)
    {
        var value = GetValue(instance);

        if(value is string stringValue)
        {
            return stringValue;
        }
        else if (value is DateTime valueDate)
        {
            return GetStringFromDate(valueDate);
        }
        else if (value is bool boolValue)
        {
            return boolValue ? "True" : "False";
        }
        else if(value is List<string> listString)
        {
            return String.Join("\n", listString);
        }

        return value?.ToString() ?? "null";
    }

    public static string GetStringFromDate(DateTime date)
    {
        var cultureInfo = CultureInfo.GetCultureInfo("es-ES");
        return date.ToString("yyyy/MM/dd", cultureInfo);
    }
}
