using MRA.DTO.Enums.Drawing;

namespace MRA.DTO.ViewModels.Art.Select
{
    public class ProductListItem
    {
        public string ProductName { get; set; }
        public DrawingProductTypes ProductTypeId { get; set; }
        public string ProductType { get; set; }
    }
}
