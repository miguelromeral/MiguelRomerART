using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Configuration.Options;

public class AzureStorageOptions
{
    public string ConnectionString { get; set; }
    public string BlobStorageContainer { get; set; }
    public string BlobPath { get; set; }
    public string ExportLocation { get; set; }
}
