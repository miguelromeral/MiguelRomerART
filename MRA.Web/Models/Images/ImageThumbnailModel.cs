using MRA.DTO.Firebase.Models;

namespace MRA.Web.Models.Images
{
    public class ImageThumbnailModel
    {
        public Drawing Drawing { get; }
        public bool FullSize { get; set; }

        public ImageThumbnailModel(Drawing drawing)
        {
            Drawing = drawing;
        }
    }
}
