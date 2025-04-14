using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MRA.DTO.Logger;
using MRA.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Logger
{
    public class MRLoggerProvider : ILoggerProvider
    {
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appConfig;

        public MRLoggerProvider(IConfiguration configuration, AppSettings appConfig)
        {
            _configuration = configuration;
            _appConfig = appConfig;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MRLogger(_appConfig);
        }

        public void Dispose() { }
    }
}
