using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MRA.DTO.Firebase.Models;
#nullable disable
namespace MRA.Web.Models.Admin
{
    public class EditCollectionsViewModel
    {
        public const string SEPARADOR_DRAWING_ID = ";";

        public List<Drawing> ListDrawings;
        public Collection Collection;
        public string DrawingsId;

        public static string[] ArrayDrawingsId(string drawingsId) { 
            return drawingsId?.Split(SEPARADOR_DRAWING_ID);
        }

        public bool IsEditing { get { return !string.IsNullOrEmpty(Collection?.Id ?? ""); } }

    }
}
