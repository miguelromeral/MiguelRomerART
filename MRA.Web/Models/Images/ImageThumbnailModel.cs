using MRA.Services.Firebase.Models;

namespace MRA.Web.Models.Images
{
    public class ImageThumbnailModel
    {
        public Drawing Drawing { get; }

        public ImageThumbnailModel(Drawing drawing)
        {
            Drawing = drawing;
        }
    }
}
