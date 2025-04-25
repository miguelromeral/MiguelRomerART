using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MRA.Infrastructure.Storage;

public static class ImageConverter
{
    public static Image ResizeImageIfNecessary(this Image image, int width)
    {
        if (width > 0)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width, 0),
                Mode = ResizeMode.Max
            }));
        }

        return image;
    }

    public static async Task<Image> LoadImageAsync(this string pathToImage) => await Image.LoadAsync(pathToImage);
    public static async Task<Image> LoadImageAsync(this MemoryStream memoryStream) => await Image.LoadAsync(memoryStream);
}
