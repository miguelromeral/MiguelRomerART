
namespace MRA.Infrastructure.Excel.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ExcelColumnAttribute : Attribute
{
    public string Name { get; }
    public int Order { get; }

    public bool URL { get; }
    public bool IgnoreOnImport { get; }
    public bool WrapText { get; }
    public int Width { get; }

    public ExcelColumnAttribute(
        string name,
        int order,
        int width = 40,
        bool url = false,
        bool ignoreOnImport = false,
        bool wrapText = false)
    {
        Name = name;
        Order = order;
        Width = width;
        URL = url;
        IgnoreOnImport = ignoreOnImport;
        WrapText = wrapText;
    }
}
