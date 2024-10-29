
using MRA.DTO.Firebase.Models;

namespace MRA.DTO.Excel.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ExcelColumnAttribute : Attribute
{
    public string Name { get; }
    public int Order { get; }

    public bool WrapText = false;
    public bool Hidden = false;

    public ExcelColumnAttribute(string name, int order, bool wrapText = false, bool hidden = false)
    {
        Name = name;
        Order = order;
        WrapText = wrapText;
        Hidden = hidden;
    }
}
