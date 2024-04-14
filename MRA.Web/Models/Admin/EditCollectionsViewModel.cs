using MRA.Services.Firebase.Models;

namespace MRA.Web.Models.Admin
{
    public class EditCollectionsViewModel
    {
        public Collection Collection;
        public bool IsEditing { get { return !string.IsNullOrEmpty(Collection?.Id ?? ""); } }
    }
}
