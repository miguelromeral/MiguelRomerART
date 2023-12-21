using Microsoft.AspNetCore.Mvc.RazorPages;
using MRA.Services.Firebase.Interfaces;
using MRA.Services;
using MRA.Services.Firebase.Models;

namespace MRA.Web.Models
{
    public class IndexModel : PageModel
    {
        private readonly IFirestoreService _firestoreService;

        public List<Drawing>? Drawings;
        public List<BlobFileInfo> Blobs;
        public string BlobURL;

        public IndexModel(IFirestoreService firestoreService)
        {
            _firestoreService = firestoreService;
        }

        public async Task OnGetAsync()
        {
            Drawings = await _firestoreService.GetAll();
        }
    }
}
