using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Options;

public class MRALoggerOptions
{
    public string Location { get; set; }
    public string FilePrefix { get; set; }
    public string DateNameFormat { get; set; }
    public string DateFormat { get; set; }
}
