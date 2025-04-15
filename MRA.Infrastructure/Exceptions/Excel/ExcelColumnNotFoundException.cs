namespace MRA.Infrastructure.Exceptions.Excel;

public class ExcelColumnNotFoundException : Exception
{
    public ExcelColumnNotFoundException()
    {
    }

    public ExcelColumnNotFoundException(string message)
        : base(message)
    {
    }

    public ExcelColumnNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
