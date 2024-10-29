
using MRA.DTO.Firebase.Models;

namespace MRA.DTO.Excel.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ExcelColumnAttribute : Attribute
{
    public string Name { get; }
    public int Order { get; }

    public bool URL = false;
    public bool Hidden = false;

    public ExcelColumnAttribute(string name, int order, bool hidden = false, bool url = false)
    {
        Name = name;
        Order = order;
        Hidden = hidden;
        URL = url;
    }
}
