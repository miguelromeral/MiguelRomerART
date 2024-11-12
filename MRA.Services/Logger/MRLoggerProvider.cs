using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MRA.DTO.Logger;
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

        public MRLoggerProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MRLogger(_configuration);
        }

        public void Dispose() { }
    }
}
