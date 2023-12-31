using Microsoft.AspNetCore.Mvc.RazorPages;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.Models;
using MRA.Services.AzureStorage;

namespace MRA.Web.Models
{
    public class IndexModel : PageModel
    {
        public List<Drawing>? Drawings;

        public List<string> ProductNames;
    }
}
