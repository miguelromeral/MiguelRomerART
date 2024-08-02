using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.ViewModels.Art
{
    public class UploadAzureImageRequest
    {
        public int Size { get; set; }
        public string Path { get; set; }
        public IFormFile Image { get; set; }
    }
}
