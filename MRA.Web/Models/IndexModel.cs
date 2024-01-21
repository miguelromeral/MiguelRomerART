using Microsoft.AspNetCore.Mvc.RazorPages;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.Models;
using MRA.Services.AzureStorage;

namespace MRA.Web.Models
{
    public class IndexModel : PageModel
    {
        public List<Drawing>? Drawings;

        public Dictionary<string, int> ProductNameSelect { get; set; }
        public Dictionary<string, int> CharacterNameSelect { get; set; }
        public List<string> ModelNameSelect { get; set; }
        public List<Collection> ListCollections { get; set; }

        public string Query_TextQuery { get; set; }
        public string Query_Type { get; set; }
        public string Query_ProductType { get; set; }
        public string Query_ProductName { get; set; }
        public string Query_Collection { get; set; }
        public string Query_CharacterName { get; set; }
        public string Query_ModelName { get; set; }
        public string Query_Software { get; set; }
        public string Query_Paper { get; set; }
        public string Query_Spotify { get; set; }
        public bool Query_Favorites { get; set; }
        public string Query_Sortby { get; set; }
    }
}
