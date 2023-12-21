using Microsoft.AspNetCore.Mvc.RazorPages;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase;
using MRA.Services;

namespace MRA.Web.Models
{
    public class IndexModel : PageModel
    {
        private readonly IFirestoreService _firestoreService;

        public List<Shoe>? Shoes;
        public List<BlobFileInfo> Blobs;

        public IndexModel(IFirestoreService firestoreService)
        {
            _firestoreService = firestoreService;
        }

        public async Task OnGetAsync()
        {
            Shoes = await _firestoreService.GetAll();
        }
    }
}
