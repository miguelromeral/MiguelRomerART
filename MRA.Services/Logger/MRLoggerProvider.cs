using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MRA.DTO.Logger;
using MRA.DTO.Configuration;
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
        private readonly AppConfiguration _appConfig;

        public MRLoggerProvider(IConfiguration configuration, AppConfiguration appConfig)
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
