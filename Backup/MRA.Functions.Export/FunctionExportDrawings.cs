using Microsoft.Azure.Functions.Worker;
using MRA.Services.Backup.Export;

namespace MRA.Functions.Excelsize;

public class FunctionExportDrawings
{
    private readonly IExportService _exportService;

    public FunctionExportDrawings(IExportService exportService)
    {
        _exportService = exportService;
    }

    [Function("FunctionExportDrawings")]
    public async Task Run(
#if DEBUG
    [TimerTrigger("0 */1 * * * *")]
#else
    [TimerTrigger("0 30 12 */1 * *")] // Every day at 12:30 UTC
#endif      
    TimerInfo myTimer)
    {
        await _exportService.ExportDrawings();
    }
}
