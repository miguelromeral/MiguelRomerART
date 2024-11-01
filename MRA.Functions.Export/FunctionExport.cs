using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MRA.Functions.Export
{
    public class FunctionExport
    {
        private readonly ILogger<FunctionExport> _logger;
        private readonly IConfiguration _configuration;

        public FunctionExport(ILogger<FunctionExport> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        [FunctionName("FunctionExport")]
        public void Run([TimerTrigger("*/10 * * * * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"EJECUCIÓN DE FUNCIÓN {DateTime.Now}");

                //var firestoreService = new FirestoreService(_configuration);
                //_logger.LogWarning("Proyecto: "+firestoreService.ProjectId);
                
                _logger.LogWarning("Proyecto: " + _configuration["Firebase:ProjectID"]);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en la ejecución de la función: {ex.Message}");
            }
        }
    }
}
