using MRA.Services.Firebase.Models;

namespace MRA.Web.Models
{
    public class DetailsSectionModel
    {
        public DetailsSectionModel()
        {
            ListaItems = new List<string>();
            Capitalize = true;
        }
        
        public string Title { get; set; }
        public bool Large { get; set; }
        public bool Tags { get; set; }
        public bool Capitalize { get; set; }
        public List<string> ListaItems { get; set; }
        public string RawText { get; set; }
    }
}
