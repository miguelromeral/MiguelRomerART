
using MRA.DTO.Firebase.Models;

namespace MRA.DTO.Excel.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ExcelColumnAttribute : Attribute
{
    public string Name { get; }
    public int Order { get; }

    public bool URL;
    public bool Hidden;
    public bool IgnoreOnImport;

    public ExcelColumnAttribute(
        string name, 
        int order, 
        bool hidden = false, 
        bool url = false, 
        bool ignoreOnImport = false)
    {
        Name = name;
        Order = order;
        Hidden = hidden;
        URL = url;
        IgnoreOnImport = ignoreOnImport;
    }
}
