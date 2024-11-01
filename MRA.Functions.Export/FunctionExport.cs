using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MRA.Functions.Export
{
    public class FunctionExport
    {
        private readonly ILogger<FunctionExport> _logger;

        public FunctionExport(ILogger<FunctionExport> logger)
        {
            _logger = logger;
        }

        [FunctionName("FunctionExport")]
        public void Run([TimerTrigger("*/15 * * * * *")]TimerInfo myTimer)
        {
            _logger.LogInformation($"EJECUCIÓN DE FUNCIÓN {DateTime.Now}");
        }
    }
}
