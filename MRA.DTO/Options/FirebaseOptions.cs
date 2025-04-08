using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Options;

public class FirebaseOptions
{
    public string CredentialsPath { get; set; }
    public string ProjectID { get; set; }
    public string Environment { get; set; }
    public string CollectionDrawings { get; set; }
    public string CollectionInspirations { get; set; }
    public string CollectionCollections { get; set; }
    public string CollectionExperience { get; set; }
}
