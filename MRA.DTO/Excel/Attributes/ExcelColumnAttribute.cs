
namespace MRA.DTO.Excel.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ExcelColumnAttribute : Attribute
{
    public string Name { get; }
    public int Order { get; }

    public ExcelColumnAttribute(string name, int order)
    {
        Name = name;
        Order = order;
    }
}
