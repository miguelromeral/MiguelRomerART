using MRA.Services.Firebase.Models;

namespace MRA.Web.Models.Admin
{
    public class EditDrawingViewModel
    {
        public bool IsEditing { get { return !string.IsNullOrEmpty(Drawing?.Id ?? ""); } }
        public Drawing Drawing { get; set; }
    }
}
