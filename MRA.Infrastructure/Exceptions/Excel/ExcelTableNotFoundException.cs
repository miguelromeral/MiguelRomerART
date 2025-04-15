namespace MRA.Infrastructure.Exceptions.Excel;

public class ExcelTableNotFoundException : Exception
{
    public ExcelTableNotFoundException(string message)
        : base(message)
    {
    }
}
